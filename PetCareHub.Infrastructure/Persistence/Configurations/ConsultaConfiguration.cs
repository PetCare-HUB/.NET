using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class ConsultaConfiguration : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.ToTable("CONSULTA");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("ID");

        builder.Property(c => c.PetId)
            .HasColumnName("PET_ID")
            .IsRequired();

        builder.Property(c => c.ClinicaId)
            .HasColumnName("CLINICA_ID")
            .IsRequired();

        builder.Property(c => c.DataConsulta)
            .HasColumnName("DATA_CONSULTA")
            .IsRequired();

        builder.Property(c => c.TipoConsulta)
            .HasColumnName("TIPO_CONSULTA")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Observacoes)
            .HasColumnName("OBSERVACOES")
            .HasMaxLength(1000);

        builder.Property(c => c.Valor)
            .HasColumnName("VALOR")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(c => c.RetornoRecomendado)
            .HasColumnName("RETORNO_RECOMENDADO")
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(c => c.Pet)
            .WithMany(p => p.Consultas)
            .HasForeignKey(c => c.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Clinica)
            .WithMany(c => c.Consultas)
            .HasForeignKey(c => c.ClinicaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.PetId);
        builder.HasIndex(c => c.ClinicaId);
        builder.HasIndex(c => c.DataConsulta);
    }
}