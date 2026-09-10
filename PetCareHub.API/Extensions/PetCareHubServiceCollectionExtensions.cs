using Microsoft.EntityFrameworkCore;
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
}