using Microsoft.EntityFrameworkCore;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence;

public class PetCareHubContext : DbContext
{
    public PetCareHubContext(DbContextOptions<PetCareHubContext> options)
        : base(options)
    {
    }

    public DbSet<Clinica> Clinicas => Set<Clinica>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Consulta> Consultas => Set<Consulta>();
    public DbSet<EventoPreventivo> EventosPreventivos => Set<EventoPreventivo>();
    public DbSet<LeituraSensor> LeiturasSensor => Set<LeituraSensor>();
    public DbSet<AlertaSaude> AlertasSaude => Set<AlertaSaude>();
    public DbSet<ScoreSaude> ScoresSaude => Set<ScoreSaude>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PetCareHubContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}