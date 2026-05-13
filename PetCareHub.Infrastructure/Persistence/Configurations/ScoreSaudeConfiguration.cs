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
            .HasColumnName("ID");

        builder.Property(s => s.PetId)
            .HasColumnName("PET_ID")
            .IsRequired();

        builder.Property(s => s.ScoreTotal)
            .HasColumnName("SCORE_TOTAL")
            .IsRequired();

        builder.Property(s => s.Categoria)
            .HasColumnName("CATEGORIA")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.DataCalculo)
            .HasColumnName("DATA_CALCULO")
            .IsRequired();

        builder.Property(s => s.Observacao)
            .HasColumnName("OBSERVACAO")
            .HasMaxLength(500);

        builder.HasOne(s => s.Pet)
            .WithMany(p => p.ScoresSaude)
            .HasForeignKey(s => s.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.PetId);
        builder.HasIndex(s => s.Categoria);
        builder.HasIndex(s => s.DataCalculo);
    }
}