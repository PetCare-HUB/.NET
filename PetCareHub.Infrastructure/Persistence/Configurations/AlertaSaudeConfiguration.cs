using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class AlertaSaudeConfiguration : IEntityTypeConfiguration<AlertaSaude>
{
    public void Configure(EntityTypeBuilder<AlertaSaude> builder)
    {
        builder.ToTable("ALERTA_SAUDE");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("ID");

        builder.Property(a => a.PetId)
            .HasColumnName("PET_ID")
            .IsRequired();

        builder.Property(a => a.LeituraSensorId)
            .HasColumnName("LEITURA_SENSOR_ID");

        builder.Property(a => a.TipoAlerta)
            .HasColumnName("TIPO_ALERTA")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.Nivel)
            .HasColumnName("NIVEL")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Resolvido)
            .HasColumnName("RESOLVIDO")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.DataAlerta)
            .HasColumnName("DATA_ALERTA")
            .IsRequired();

        builder.Property(a => a.DataResolucao)
            .HasColumnName("DATA_RESOLUCAO");

        builder.HasOne(a => a.Pet)
            .WithMany(p => p.AlertasSaude)
            .HasForeignKey(a => a.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.LeituraSensor)
            .WithMany(l => l.AlertasSaude)
            .HasForeignKey(a => a.LeituraSensorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => a.PetId);
        builder.HasIndex(a => a.Resolvido);
        builder.HasIndex(a => a.Nivel);
        builder.HasIndex(a => a.DataAlerta);
    }
}