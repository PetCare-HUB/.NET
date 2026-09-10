using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class LeituraColeiraConfiguration : IEntityTypeConfiguration<LeituraColeira>
{
    public void Configure(EntityTypeBuilder<LeituraColeira> builder)
    {
        builder.ToTable("LEITURA_COLEIRA");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("ID_LEITURA_COLEIRA")
            .HasDefaultValueSql("SEQ_LEITURA_COLEIRA.NEXTVAL")
            .ValueGeneratedOnAdd();

        builder.Property(l => l.PetId)
            .HasColumnName("ID_PET")
            .IsRequired();

        builder.Property(l => l.StatusAtividade)
            .HasColumnName("STATUS_ATIVIDADE")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(l => l.NivelBateria)
            .HasColumnName("NIVEL_BATERIA")
            .IsRequired();

        builder.Property(l => l.TimestampLeitura)
            .HasColumnName("TIMESTAMP_LEITURA")
            .IsRequired();

        builder.HasOne(l => l.Pet)
            .WithMany(p => p.LeiturasColeira)
            .HasForeignKey(l => l.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.PetId);
        builder.HasIndex(l => l.TimestampLeitura);
    }
}
