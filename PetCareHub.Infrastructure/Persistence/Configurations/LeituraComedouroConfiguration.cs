using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class LeituraComedouroConfiguration : IEntityTypeConfiguration<LeituraComedouro>
{
    public void Configure(EntityTypeBuilder<LeituraComedouro> builder)
    {
        builder.ToTable("LEITURA_COMEDOURO");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("ID_LEITURA_COMEDOURO")
            .HasDefaultValueSql("SEQ_LEITURA_COMEDOURO.NEXTVAL")
            .ValueGeneratedOnAdd();

        builder.Property(l => l.PetId)
            .HasColumnName("ID_PET")
            .IsRequired();

        builder.Property(l => l.NivelRacaoPct)
            .HasColumnName("NIVEL_RACAO_PCT")
            .IsRequired();

        builder.Property(l => l.PesoConsumidoG)
            .HasColumnName("PESO_CONSUMIDO_G")
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(l => l.TimestampLeitura)
            .HasColumnName("TIMESTAMP_LEITURA")
            .IsRequired();

        builder.HasOne(l => l.Pet)
            .WithMany(p => p.LeiturasComedouro)
            .HasForeignKey(l => l.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.PetId);
        builder.HasIndex(l => l.TimestampLeitura);
    }
}
