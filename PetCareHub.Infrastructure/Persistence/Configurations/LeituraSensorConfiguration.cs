using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class LeituraSensorConfiguration : IEntityTypeConfiguration<LeituraSensor>
{
    public void Configure(EntityTypeBuilder<LeituraSensor> builder)
    {
        builder.ToTable("LEITURA_SENSOR");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("ID_LEITURA");

        builder.Property(l => l.PetId)
            .HasColumnName("ID_PET")
            .IsRequired();

        builder.Property(l => l.DispositivoId)
            .HasColumnName("ID_DISPOSITIVO")
            .IsRequired();

        builder.Property(l => l.TipoLeitura)
            .HasColumnName("TIPO_LEITURA")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(l => l.Valor)
            .HasColumnName("VALOR")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(l => l.Unidade)
            .HasColumnName("UNIDADE")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(l => l.DataLeitura)
            .HasColumnName("DATA_LEITURA")
            .IsRequired();

        builder.Property(l => l.StatusLeitura)
            .HasColumnName("STATUS_LEITURA")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(l => l.Pet)
            .WithMany(p => p.LeiturasSensor)
            .HasForeignKey(l => l.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.PetId);
        builder.HasIndex(l => l.TipoLeitura);
        builder.HasIndex(l => l.DataLeitura);
    }
}