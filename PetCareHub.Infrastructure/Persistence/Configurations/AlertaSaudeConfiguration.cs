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
            .HasColumnName("ID_ALERTA");

        builder.Property(a => a.PetId)
            .HasColumnName("ID_PET")
            .IsRequired();

        builder.Property(a => a.LeituraId)
            .HasColumnName("ID_LEITURA");

        builder.Property(a => a.TipoAlerta)
            .HasColumnName("TIPO_ALERTA")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.NivelAlerta)
            .HasColumnName("NIVEL_ALERTA")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Mensagem)
            .HasColumnName("MENSAGEM")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.ValorDetectado)
            .HasColumnName("VALOR_DETECTADO")
            .HasPrecision(10, 2);

        builder.Property(a => a.LimiteReferencia)
            .HasColumnName("LIMITE_REFERENCIA")
            .HasPrecision(10, 2);

        builder.Property(a => a.Resolvido)
            .HasColumnName("RESOLVIDO")
            .HasConversion(
                resolvido => resolvido ? "S" : "N",
                valor => valor == "S"
            )
            .HasMaxLength(1)
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
            .HasForeignKey(a => a.LeituraId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => a.PetId);
        builder.HasIndex(a => a.Resolvido);
        builder.HasIndex(a => a.NivelAlerta);
        builder.HasIndex(a => a.DataAlerta);
    }
}