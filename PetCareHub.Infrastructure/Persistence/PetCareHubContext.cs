using Microsoft.EntityFrameworkCore;
using PetCareHub.Domain.Entities;

using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.Infrastructure.Persistence;

public class PetCareHubContext : DbContext
{
    private readonly ICurrentClinicContext _currentClinicContext;

    public PetCareHubContext(
        DbContextOptions<PetCareHubContext> options,
        ICurrentClinicContext currentClinicContext)
        : base(options)
    {
        _currentClinicContext = currentClinicContext;
    }

    // Sem claim válida, nenhuma linha de clínica é visível.
    public long ClinicaIdAtual => _currentClinicContext.ClinicaId ?? long.MinValue;

    /// <summary>
    /// Ponto de entrada das consultas dos repositórios: aplica o filtro por
    /// clínica, exceto para o admin da PetCare Hub, que enxerga a rede toda.
    /// </summary>
    /// <remarks>
    /// A decisão é tomada aqui em C#, escolhendo entre duas consultas, e não
    /// como uma condição dentro do HasQueryFilter. O provider Oracle traduz
    /// uma condição booleana constante como o literal FALSE, que não existe
    /// em SQL no Oracle 19c e estoura ORA-00904 em toda listagem.
    /// </remarks>
    public IQueryable<TEntity> Consultar<TEntity>() where TEntity : class =>
        _currentClinicContext.EhAdmin
            ? Set<TEntity>().IgnoreQueryFilters()
            : Set<TEntity>();

    public DbSet<Tutor> Tutores { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
    public DbSet<AlertaSaude> AlertasSaude { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Clinica> Clinicas { get; set; }
    public DbSet<ScoreSaude> ScoresSaude { get; set; }
    public DbSet<EventoPreventivo> EventosPreventivos { get; set; }
    public DbSet<ProtocoloPreventivo> ProtocolosPreventivos { get; set; }
    public DbSet<LeituraColeira> LeiturasColeira { get; set; }
    public DbSet<LeituraComedouro> LeiturasComedouro { get; set; }
    public DbSet<LeituraAmbiente> LeiturasAmbiente { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Sequences existentes no banco Oracle (schema é gerenciado pelo Flyway do Java — o
        // .NET nunca roda migration própria aqui, só mapeia o que já existe).
        modelBuilder.HasSequence<long>("SEQ_TUTOR");
        modelBuilder.HasSequence<long>("SEQ_CLINICA");
        modelBuilder.HasSequence<long>("SEQ_PET");
        modelBuilder.HasSequence<long>("SEQ_CONSULTA");
        modelBuilder.HasSequence<long>("SEQ_EVENTO_PREVENTIVO");
        modelBuilder.HasSequence<long>("SEQ_PROTOCOLO_PREVENTIVO");
        modelBuilder.HasSequence<long>("SEQ_LEITURA_COLEIRA");
        modelBuilder.HasSequence<long>("SEQ_LEITURA_COMEDOURO");
        modelBuilder.HasSequence<long>("SEQ_LEITURA_AMBIENTE");
        modelBuilder.HasSequence<long>("SEQ_ALERTA_SAUDE");
        modelBuilder.HasSequence<long>("SEQ_SCORE_SAUDE");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PetCareHubContext).Assembly);

        modelBuilder.Entity<Clinica>()
            .HasQueryFilter(clinica => clinica.Id == ClinicaIdAtual);
        modelBuilder.Entity<Pet>()
            .HasQueryFilter(pet => pet.ClinicaId == ClinicaIdAtual);
        modelBuilder.Entity<Consulta>()
            .HasQueryFilter(consulta => consulta.ClinicaId == ClinicaIdAtual);
        modelBuilder.Entity<Tutor>()
            .HasQueryFilter(tutor => tutor.Pets.Any(pet => pet.ClinicaId == ClinicaIdAtual));
        modelBuilder.Entity<AlertaSaude>()
            .HasQueryFilter(alerta => alerta.Pet != null && alerta.Pet.ClinicaId == ClinicaIdAtual);
        modelBuilder.Entity<ScoreSaude>()
            .HasQueryFilter(score => score.Pet != null && score.Pet.ClinicaId == ClinicaIdAtual);
        modelBuilder.Entity<EventoPreventivo>()
            .HasQueryFilter(evento => evento.Pet != null && evento.Pet.ClinicaId == ClinicaIdAtual);
        modelBuilder.Entity<LeituraColeira>()
            .HasQueryFilter(leitura => leitura.Pet != null && leitura.Pet.ClinicaId == ClinicaIdAtual);
        modelBuilder.Entity<LeituraComedouro>()
            .HasQueryFilter(leitura => leitura.Pet != null && leitura.Pet.ClinicaId == ClinicaIdAtual);
        modelBuilder.Entity<LeituraAmbiente>()
            .HasQueryFilter(leitura => leitura.Pet != null && leitura.Pet.ClinicaId == ClinicaIdAtual);

        base.OnModelCreating(modelBuilder);
    }
}
