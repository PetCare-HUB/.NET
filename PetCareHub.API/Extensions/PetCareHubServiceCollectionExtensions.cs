using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Implementations;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Infrastructure.Persistence;
using PetCareHub.Infrastructure.Persistence.Repositories;

namespace PetCareHub.API.Extensions;

public static class PetCareHubServiceCollectionExtensions
{
    public static IServiceCollection AddPetCareHubRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IConsultaRepository, ConsultaRepository>();
        services.AddScoped<IAlertaSaudeRepository, AlertaSaudeRepository>();
        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IClinicaRepository, ClinicaRepository>();
        services.AddScoped<IScoreSaudeRepository, ScoreSaudeRepository>();

        return services;
    }
    public static IServiceCollection AddPetCareHubApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IConsultaService, ConsultaService>();
        services.AddScoped<IAlertaSaudeService, AlertaSaudeService>();
        services.AddScoped<ITutorService, TutorService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IClinicaService, ClinicaService>();
        services.AddScoped<IScoreSaudeService, ScoreSaudeService>();

        return services;
    }
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

    /// <summary>
    /// Valida os mesmos tokens JWT (RS256) emitidos pela API Java — o .NET nunca emite
    /// token, só confia na assinatura RSA e nas claims (role, clinicaId/tutorId) de quem
    /// já autenticou no Java. A chave pública vem de <c>Jwt:PublicKeyPem</c> (útil para
    /// injetar via variável de ambiente <c>Jwt__PublicKeyPem</c> em produção, com o mesmo
    /// valor configurado como RSA_PUBLIC_KEY no Java) ou, na ausência dela, do arquivo
    /// <c>Keys/public_key.pem</c> commitado (chave de desenvolvimento/teste, a mesma usada
    /// pela suíte de testes do Java — nunca a chave real de produção).
    /// </summary>
    public static IServiceCollection AddPetCareHubAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var publicKeyPem = configuration["Jwt:PublicKeyPem"];
        if (string.IsNullOrWhiteSpace(publicKeyPem))
        {
            var keyPath = Path.Combine(AppContext.BaseDirectory, "Keys", "public_key.pem");
            publicKeyPem = File.ReadAllText(keyPath);
        }

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
}