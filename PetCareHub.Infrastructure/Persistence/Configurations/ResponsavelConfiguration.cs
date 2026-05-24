using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class ResponsavelConfiguration : IEntityTypeConfiguration<Responsavel>
{
    public void Configure(EntityTypeBuilder<Responsavel> builder)
    {
        builder.ToTable("RESPONSAVEL");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("ID_RESPONSAVEL")
            .HasDefaultValueSql("SEQ_RESPONSAVEL.NEXTVAL")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Email)
            .HasColumnName("EMAIL")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(r => r.Telefone)
            .HasColumnName("TELEFONE")
            .HasMaxLength(20);

        builder.Property(r => r.Cpf)
            .HasColumnName("CPF")
            .HasMaxLength(11);

        builder.Property(r => r.DataCadastro)
            .HasColumnName("DATA_CADASTRO")
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(r => r.Ativo)
            .HasColumnName("ATIVO")
            .HasConversion(
                ativo => ativo ? "S" : "N",
                valor => valor == "S"
            )
            .HasMaxLength(1)
            .IsRequired();

        builder.HasIndex(r => r.Email).IsUnique();
        builder.HasIndex(r => r.Cpf).IsUnique();
    }
}