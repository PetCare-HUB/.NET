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
    public DbSet<LeituraSensor> LeiturasSensor { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Sequences existentes no banco Oracle
        modelBuilder.HasSequence<long>("SEQ_RESPONSAVEL");
        modelBuilder.HasSequence<long>("SEQ_CLINICA");
        modelBuilder.HasSequence<long>("SEQ_PET");
        modelBuilder.HasSequence<long>("SEQ_CONSULTA");
        modelBuilder.HasSequence<long>("SEQ_EVENTO_PREVENTIVO");
        modelBuilder.HasSequence<long>("SEQ_LEITURA_SENSOR");
        modelBuilder.HasSequence<long>("SEQ_ALERTA_SAUDE");
        modelBuilder.HasSequence<long>("SEQ_SCORE_SAUDE");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PetCareHubContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}