using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class ScoreSaudeConfiguration : IEntityTypeConfiguration<ScoreSaude>
{
    public void Configure(EntityTypeBuilder<ScoreSaude> builder)
    {
        builder.ToTable("SCORE_SAUDE");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("ID_SCORE");

        builder.Property(s => s.PetId)
            .HasColumnName("ID_PET")
            .IsRequired();

        builder.Property(s => s.ScoreTotal)
            .HasColumnName("SCORE_TOTAL")
            .IsRequired();

        builder.Property(s => s.ScoreAtividade)
            .HasColumnName("SCORE_ATIVIDADE")
            .IsRequired();

        builder.Property(s => s.ScoreAlimentacao)
            .HasColumnName("SCORE_ALIMENTACAO")
            .IsRequired();

        builder.Property(s => s.ScoreAmbiente)
            .HasColumnName("SCORE_AMBIENTE")
            .IsRequired();

        builder.Property(s => s.ScoreConsulta)
            .HasColumnName("SCORE_CONSULTA")
            .IsRequired();

        builder.Property(s => s.ScorePreventivo)
            .HasColumnName("SCORE_PREVENTIVO")
            .IsRequired();

        builder.Property(s => s.Categoria)
            .HasColumnName("CATEGORIA")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.DataCalculo)
            .HasColumnName("DATA_CALCULO")
            .IsRequired();

        builder.HasOne(s => s.Pet)
            .WithMany(p => p.ScoresSaude)
            .HasForeignKey(s => s.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.PetId);
        builder.HasIndex(s => s.Categoria);
        builder.HasIndex(s => s.DataCalculo);
    }
}