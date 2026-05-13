using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetCareHub.Infrastructure.Persistence;

namespace PetCareHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OracleConnection");

        services.AddDbContext<PetCareHubContext>(options =>
        {
            options.UseOracle(connectionString);
        });

        return services;
    }
}