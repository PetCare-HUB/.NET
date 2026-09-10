using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class LeituraAmbienteConfiguration : IEntityTypeConfiguration<LeituraAmbiente>
{
    public void Configure(EntityTypeBuilder<LeituraAmbiente> builder)
    {
        builder.ToTable("LEITURA_AMBIENTE");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("ID_LEITURA_AMBIENTE")
            .HasDefaultValueSql("SEQ_LEITURA_AMBIENTE.NEXTVAL")
            .ValueGeneratedOnAdd();

        builder.Property(l => l.PetId)
            .HasColumnName("ID_PET")
            .IsRequired();

        builder.Property(l => l.TemperaturaAmbiente)
            .HasColumnName("TEMPERATURA_AMBIENTE")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(l => l.UmidadePct)
            .HasColumnName("UMIDADE_PCT")
            .IsRequired();

        builder.Property(l => l.QualidadeArPpm)
            .HasColumnName("QUALIDADE_AR_PPM")
            .IsRequired();

        builder.Property(l => l.PetPresente)
            .HasColumnName("PET_PRESENTE")
            .IsRequired();

        builder.Property(l => l.TimestampLeitura)
            .HasColumnName("TIMESTAMP_LEITURA")
            .IsRequired();

        builder.HasOne(l => l.Pet)
            .WithMany(p => p.LeiturasAmbiente)
            .HasForeignKey(l => l.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.PetId);
        builder.HasIndex(l => l.TimestampLeitura);
    }
}
