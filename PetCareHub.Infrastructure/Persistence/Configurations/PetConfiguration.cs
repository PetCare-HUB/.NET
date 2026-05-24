using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("PET");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("ID_PET")
            .HasDefaultValueSql("SEQ_PET.NEXTVAL")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.ResponsavelId)
            .HasColumnName("ID_RESPONSAVEL")
            .IsRequired();

        builder.Property(p => p.ClinicaId)
            .HasColumnName("ID_CLINICA")
            .IsRequired();

        builder.Property(p => p.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(p => p.Especie)
            .HasColumnName("ESPECIE")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Raca)
            .HasColumnName("RACA")
            .HasMaxLength(80);

        builder.Property(p => p.DataNascimento)
            .HasColumnName("DATA_NASCIMENTO")
            .HasColumnType("DATE");

        builder.Property(p => p.PesoKg)
            .HasColumnName("PESO_KG")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(p => p.Sexo)
            .HasColumnName("SEXO")
            .HasMaxLength(1);

        builder.Property(p => p.CondicoesCronicas)
            .HasColumnName("CONDICOES_CRONICAS")
            .HasMaxLength(300);

        builder.Property(p => p.DataCadastro)
            .HasColumnName("DATA_CADASTRO")
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(p => p.Ativo)
            .HasColumnName("ATIVO")
            .HasConversion(
                ativo => ativo ? "S" : "N",
                valor => valor == "S"
            )
            .HasMaxLength(1)
            .IsRequired();

        builder.HasOne(p => p.Clinica)
            .WithMany(c => c.Pets)
            .HasForeignKey(p => p.ClinicaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Responsavel)
            .WithMany(r => r.Pets)
            .HasForeignKey(p => p.ResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.ClinicaId);
        builder.HasIndex(p => p.ResponsavelId);
        builder.HasIndex(p => p.Especie);
    }
}