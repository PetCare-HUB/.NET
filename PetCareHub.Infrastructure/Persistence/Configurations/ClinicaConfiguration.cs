using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class ClinicaConfiguration : IEntityTypeConfiguration<Clinica>
{
    public void Configure(EntityTypeBuilder<Clinica> builder)
    {
        builder.ToTable("CLINICA");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("ID");

        builder.Property(c => c.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(c => c.Cnpj)
            .HasColumnName("CNPJ")
            .HasMaxLength(18)
            .IsRequired();

        builder.Property(c => c.Endereco)
            .HasColumnName("ENDERECO")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Telefone)
            .HasColumnName("TELEFONE")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasColumnName("EMAIL")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(c => c.DataCadastro)
            .HasColumnName("DATA_CADASTRO")
            .IsRequired();

        builder.Property(c => c.Ativa)
            .HasColumnName("ATIVA")
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(c => c.Cnpj)
            .IsUnique();
    }
}