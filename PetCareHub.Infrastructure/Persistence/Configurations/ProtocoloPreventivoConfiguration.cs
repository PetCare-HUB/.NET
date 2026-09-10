using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class ProtocoloPreventivoConfiguration : IEntityTypeConfiguration<ProtocoloPreventivo>
{
    public void Configure(EntityTypeBuilder<ProtocoloPreventivo> builder)
    {
        builder.ToTable("PROTOCOLO_PREVENTIVO");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("ID_PROTOCOLO")
            .HasDefaultValueSql("SEQ_PROTOCOLO_PREVENTIVO.NEXTVAL")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Especie)
            .HasColumnName("ESPECIE")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Raca)
            .HasColumnName("RACA")
            .HasMaxLength(80);

        builder.Property(p => p.TipoEvento)
            .HasColumnName("TIPO_EVENTO")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.IdadeMesesRecomendada)
            .HasColumnName("IDADE_MESES_RECOMENDADA");

        builder.Property(p => p.IntervaloDias)
            .HasColumnName("INTERVALO_DIAS");

        builder.Property(p => p.Ativo)
            .HasColumnName("ATIVO")
            .HasConversion(
                ativo => ativo ? "S" : "N",
                valor => valor == "S"
            )
            .HasMaxLength(1)
            .IsRequired();

        builder.HasIndex(p => p.Especie);
    }
}
