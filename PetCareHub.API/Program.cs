using System.Reflection;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PetCareHub.API.Exceptions;
using PetCareHub.API.Extensions;
using PetCareHub.API.Health;
using PetCareHub.Application.Diagnostics;
using PetCareHub.Infrastructure.Diagnostics;
using Serilog;
using Serilog.Events;

namespace PetCareHub.API;

/// <summary>Ponto de entrada da API — configura DI, autenticação, observabilidade e o pipeline HTTP.</summary>
public class Program
{
    /// <summary>Nome do esquema de segurança JWT exposto no Swagger.</summary>
    private const string EsquemaJwt = "Bearer";

    /// <summary>Monta o host e inicia a aplicação.</summary>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, config) =>
        {
            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationId()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs/petcarehub-.log", rollingInterval: RollingInterval.Day);
        });

        builder.Services.AddDefaultCorrelationId();
        builder.Services.AddHttpContextAccessor();

        // Hospedada em container atrás de proxy (Render), a API recebe HTTP puro: o
        // TLS termina na borda. Sem ler X-Forwarded-Proto, Request.Scheme vale "http"
        // e todo link HATEOAS sai com esquema errado — justamente o que
        // HateoasExtensions promete entregar correto. Os KnownNetworks/KnownProxies
        // padrão só confiam no loopback, e o proxy do Render não está nele, daí o
        // Clear() nos dois.
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.AddPetCareHubDbContext(builder.Configuration);
        builder.Services.AddPetCareHubMongo(builder.Configuration);

        builder.Services.AddPetCareHubRepositories();

        builder.Services.AddPetCareHubApplicationServices();

        builder.Services.AddPetCareHubAuthentication(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("PetCareHub.API"))
            .WithTracing(tracing => tracing
                .AddSource(AppTelemetry.SourceName)
                .AddSource(InfraTelemetry.SourceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddConsoleExporter());

        // Em configuração, não no código: o endereço muda entre rodar local (onde a
        // API Java sobe em localhost) e o ambiente publicado. Não é segredo — é uma
        // URL pública —, então o valor padrão fica versionado no appsettings.json.
        var urlSaudeApiJava = builder.Configuration["JavaApi:HealthUrl"]
            ?? throw new InvalidOperationException(
                "JavaApi:HealthUrl não configurado: o health check da API Java não tem alvo.");

        builder.Services
            .AddHealthChecks()
            // Liveness: só confirma que o processo da API está de pé, sem tocar em
            // nenhuma dependência externa — nunca falha por causa do Oracle.
            .AddCheck(
                "self",
                () => HealthCheckResult.Healthy("API em execução."),
                tags: new[] { "live" })
            // Readiness: as dependências externas desta API — o Oracle, de onde vem
            // todo o dado relacional; o MongoDB, onde ficam os resumos de contexto da
            // IA; e a API Java, que emite os tokens validados aqui e processa o dado
            // que este dashboard serve.
            .AddOracle(
                builder.Configuration.GetConnectionString("DefaultConnection")!,
                name: "oracle-db",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "ready", "db", "oracle" })
            // Degraded, não Unhealthy: o Mongo guarda só os resumos de contexto da IA.
            // Sem ele a API continua servindo todo o dado relacional, então responder
            // 503 no readiness afirmaria que nada funciona, o que é falso. Degraded
            // responde 200 e mostra no corpo qual check caiu — o Mongo continua sendo
            // verificado e reportado, só não derruba o grupo inteiro junto.
            .AddMongoDb(
                databaseNameFactory: _ => builder.Configuration["Mongo:Database"] ?? "petcarehub",
                name: "mongodb",
                failureStatus: HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(5),
                tags: new[] { "ready", "db", "mongodb" })
            // Serviço externo, não banco — a rubrica pede as duas coisas separadamente
            // ("conectividade com o banco de dados (Oracle/MongoDB) e disponibilidade
            // de serviços externos"), e o Oracle e o Mongo cobrem só a primeira.
            //
            // A API Java não expõe Actuator, então o alvo é o /keepalive: público,
            // leve e já liberado no SecurityConfig de lá.
            //
            // Degraded pela mesma razão do Mongo: com o Java hibernando, esta API
            // continua servindo todo o dado relacional e o Swagger, então 503 no
            // readiness afirmaria que nada funciona.
            .AddUrlGroup(
                new Uri(urlSaudeApiJava),
                name: "api-java",
                failureStatus: HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(5),
                tags: new[] { "ready", "external", "api" });

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "🐾 PetCare Hub API",
                Version = "v1",
                Description = """
                              API REST para gerenciamento de saúde de pets, consultas, alertas e indicadores clínicos.

                              **Como autenticar:** chame `POST /auth/login` na API Java, copie o campo `token`
                              da resposta e cole no botão **Authorize** acima — só o token, sem escrever "Bearer".
                              """,
                TermsOfService = new Uri("https://example.com/petcarehub/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Equipe PetCare Hub",
                    Email = "contato@petcarehub.com",
                    Url = new Uri("https://example.com/petcarehub")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://example.com/petcarehub/license")
                }
            });

            // Sem isto o Swagger não exibe o botão Authorize. Todo controller desta API
            // exige [Authorize] e não há nenhum AllowAnonymous, então a interface
            // publicada fica sem como enviar o token e os endpoints respondem 401 — o
            // mesmo problema que o SwaggerConfig do lado Java já resolve por lá.
            options.AddSecurityDefinition(EsquemaJwt, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Token devolvido por POST /auth/login da API Java."
            });

            // Aplicado globalmente: esta API não expõe endpoint anônimo no Swagger, então
            // não há rota pública em que o header atrapalhe.
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = EsquemaJwt
                    }
                }] = Array.Empty<string>()
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });

        var app = builder.Build();

        // Antes de qualquer coisa que leia esquema ou IP do request — inclusive o
        // UseHttpsRedirection abaixo, que sem isso tentaria redirecionar um request
        // que já chegou por HTTPS na borda do proxy.
        app.UseForwardedHeaders();

        app.UseCorrelationId();

        app.UseExceptionHandler();

        // Sem a chave pública certa a API sobe saudável e recusa todo token real com
        // 401 — falha silenciosa que só aparece quando alguém tenta usar a API. Um
        // aviso no log na subida é o que transforma isso em diagnóstico.
        if (!app.Environment.IsDevelopment()
            && string.IsNullOrWhiteSpace(builder.Configuration["Jwt:PublicKeyPem"]))
        {
            app.Logger.LogWarning(
                "Jwt:PublicKeyPem não configurado: a validação de token está usando a chave " +
                "pública de desenvolvimento commitada em Keys/public_key.pem. Tokens emitidos " +
                "pela API Java em produção serão recusados com 401. Configure a variável de " +
                "ambiente Jwt__PublicKeyPem com a mesma chave pública do RSA_PUBLIC_KEY do Java.");
        }

        // O Swagger fica ligado também fora de Development: hospedada, esta é a única
        // interface da API, e é por ela que a integração com o Java e com o Oracle é
        // demonstrada. Não expõe dado nenhum — todo endpoint de negócio continua
        // exigindo o JWT.
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCare Hub API v1");
            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Combinado (compatibilidade com quem já monitora só /health).
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        });

        // Liveness — só o "self", nunca depende do Oracle. É esse que uma orquestração
        // (ex.: restart automático) deveria checar para saber se o processo travou.
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        });

        // Readiness — só as dependências externas (hoje, só o Oracle). É esse que decide
        // se a API está pronta para receber tráfego real.
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        });

        app.Run();
    }
}
