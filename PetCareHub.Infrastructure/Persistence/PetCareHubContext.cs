using Microsoft.EntityFrameworkCore;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence;

public class PetCareHubContext : DbContext
{
    public PetCareHubContext(DbContextOptions<PetCareHubContext> options)
        : base(options)
    {
    }

    public DbSet<Responsavel> Responsaveis { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
    public DbSet<AlertaSaude> AlertasSaude { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Clinica> Clinicas { get; set; }
    public DbSet<ScoreSaude> ScoresSaude { get; set; }
    public DbSet<EventoPreventivo> EventosPreventivos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PetCareHubContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}