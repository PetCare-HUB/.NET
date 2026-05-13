using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class EventoPreventivoConfiguration : IEntityTypeConfiguration<EventoPreventivo>
{
    public void Configure(EntityTypeBuilder<EventoPreventivo> builder)
    {
        builder.ToTable("EVENTO_PREVENTIVO");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID");

        builder.Property(e => e.PetId)
            .HasColumnName("PET_ID")
            .IsRequired();

        builder.Property(e => e.Tipo)
            .HasColumnName("TIPO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(e => e.DataPrevista)
            .HasColumnName("DATA_PREVISTA")
            .IsRequired();

        builder.Property(e => e.DataRealizacao)
            .HasColumnName("DATA_REALIZACAO");

        builder.Property(e => e.Status)
            .HasColumnName("STATUS")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(e => e.Pet)
            .WithMany(p => p.EventosPreventivos)
            .HasForeignKey(e => e.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.PetId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.DataPrevista);
    }
}