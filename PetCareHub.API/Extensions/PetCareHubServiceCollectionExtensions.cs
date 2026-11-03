using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Implementations;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.API.Services;
using PetCareHub.Infrastructure.Persistence;
using PetCareHub.Infrastructure.Persistence.Repositories;

namespace PetCareHub.API.Extensions;

/// <summary>Métodos de extensão que registram os serviços da API no container de DI.</summary>
public static class PetCareHubServiceCollectionExtensions
{
    /// <summary>Registra os repositórios (Infrastructure) usados pelos Services.</summary>
    public static IServiceCollection AddPetCareHubRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IConsultaRepository, ConsultaRepository>();
        services.AddScoped<IAlertaSaudeRepository, AlertaSaudeRepository>();
        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IClinicaRepository, ClinicaRepository>();
        services.AddScoped<IScoreSaudeRepository, ScoreSaudeRepository>();
        services.AddScoped<IPetIotSummarySource, PetIotSummarySource>();
        services.AddScoped<IResumoPetRepository, ResumoPetMongoRepository>();

        return services;
    }
    /// <summary>Registra os Services de aplicação (regras de negócio).</summary>
    public static IServiceCollection AddPetCareHubApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IConsultaService, ConsultaService>();
        services.AddScoped<IAlertaSaudeService, AlertaSaudeService>();
        services.AddScoped<ITutorService, TutorService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IClinicaService, ClinicaService>();
        services.AddScoped<IScoreSaudeService, ScoreSaudeService>();
        services.AddScoped<IResumoPetService, ResumoPetService>();
        services.AddScoped<ICurrentClinicContext, HttpCurrentClinicContext>();

        return services;
    }
    /// <summary>Registra o <see cref="PetCareHubContext"/> apontando pro Oracle configurado.</summary>
    public static IServiceCollection AddPetCareHubDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' não encontrada. Configure em appsettings.json ou no ambiente.");

        services.AddDbContext<PetCareHubContext>(options =>
            options.UseOracle(connectionString));

        return services;
    }

    /// <summary>Registra a conexão com MongoDB usada para os documentos de contexto da IA.</summary>
    public static IServiceCollection AddPetCareHubMongo(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Mongo")
            ?? "mongodb://localhost:27017";
        var databaseName = configuration["Mongo:Database"] ?? "petcarehub";

        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

        return services;
    }

    /// <summary>
    /// Valida os mesmos tokens JWT (RS256) emitidos pela API Java — o .NET nunca emite
    /// token, só confia na assinatura RSA e nas claims (role, clinicaId/tutorId) de quem
    /// já autenticou no Java. A chave pública vem de <c>Jwt:PublicKeyPem</c> (útil para
    /// injetar via variável de ambiente <c>Jwt__PublicKeyPem</c> em produção, com o mesmo
    /// valor configurado como RSA_PUBLIC_KEY no Java) ou, na ausência dela, do arquivo
    /// <c>Keys/public_key.pem</c> commitado (chave de desenvolvimento/teste, a mesma usada
    /// pela suíte de testes do Java — nunca a chave real de produção).
    /// Ver <see cref="ResolverChavePublica"/> para as formas aceitas no valor.
    /// </summary>
    public static IServiceCollection AddPetCareHubAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var publicKeyPem = ResolverChavePublica(configuration);

        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Sem isso, o handler renomeia a claim "role" para a URI legada
                // ClaimTypes.Role por baixo dos panos, e o RoleClaimType abaixo (que aponta
                // pro nome real da claim emitida pelo Java) deixa de bater com nada —
                // [Authorize(Roles=...)] falha com 403 mesmo para um token válido.
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "petcare-hub-api",
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    RoleClaimType = "role",
                    NameClaimType = "sub",
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Devolve o PEM da chave pública a partir de <c>Jwt:PublicKeyPem</c>.
    /// </summary>
    /// <remarks>
    /// Aceita as mesmas duas formas que o <c>rsa.public-key</c> do Java: o conteúdo do PEM
    /// direto no valor, ou um <c>file:/caminho</c>. A segunda é a que importa hospedado —
    /// é assim que a chave chega pelos Secret Files do Render, montados em
    /// <c>/etc/secrets</c>, e evita depender de quebra de linha sobreviver dentro de uma
    /// variável de ambiente. Com isso os dois serviços recebem exatamente o mesmo valor
    /// configurado. Sem nada configurado, cai na chave de desenvolvimento commitada.
    /// </remarks>
    private static string ResolverChavePublica(IConfiguration configuration)
    {
        var configurado = configuration["Jwt:PublicKeyPem"];

        if (string.IsNullOrWhiteSpace(configurado))
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Keys", "public_key.pem"));

        const string prefixoArquivo = "file:";

        if (configurado.StartsWith(prefixoArquivo, StringComparison.OrdinalIgnoreCase))
            return File.ReadAllText(configurado[prefixoArquivo.Length..].Trim());

        return configurado;
    }
}
