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
            .HasColumnName("ID_CONSULTA");

        builder.Property(c => c.PetId)
            .HasColumnName("ID_PET")
            .IsRequired();

        builder.Property(c => c.ClinicaId)
            .HasColumnName("ID_CLINICA")
            .IsRequired();

        builder.Property(c => c.DataConsulta)
            .HasColumnName("DATA_CONSULTA")
            .IsRequired();

        builder.Property(c => c.TipoConsulta)
            .HasColumnName("TIPO_CONSULTA")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(500);

        builder.Property(c => c.Diagnostico)
            .HasColumnName("DIAGNOSTICO")
            .HasMaxLength(500);

        builder.Property(c => c.Valor)
            .HasColumnName("VALOR")
            .HasPrecision(10, 2);

        builder.Property(c => c.RetornoRecomendado)
            .HasColumnName("RETORNO_RECOMENDADO")
            .HasConversion(
                retorno => retorno ? "S" : "N",
                valor => valor == "S"
            )
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(c => c.DataRetorno)
            .HasColumnName("DATA_RETORNO");

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