using System.Reflection;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PetCareHub.API.Exceptions;
using PetCareHub.API.Extensions;
using PetCareHub.API.Health;
using Serilog;
using Serilog.Events;

namespace PetCareHub.API;

public class Program
{
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

        builder.Services.AddPetCareHubDbContext(builder.Configuration);

        builder.Services.AddPetCareHubRepositories();

        builder.Services.AddPetCareHubApplicationServices();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("PetCareHub.API"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddConsoleExporter());

        builder.Services
            .AddHealthChecks()
            .AddOracle(
                builder.Configuration.GetConnectionString("DefaultConnection")!,
                name: "oracle-db",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "oracle" });

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "🐾 PetCare Hub API",
                Version = "v1",
                Description = "API REST para gerenciamento de saúde de pets, consultas, alertas e indicadores clínicos",
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

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });

        var app = builder.Build();

        app.UseCorrelationId();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCare Hub API v1");
                options.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        });

        app.Run();
    }
}
