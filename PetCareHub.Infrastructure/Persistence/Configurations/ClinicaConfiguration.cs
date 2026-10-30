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
            .HasColumnName("ID_CLINICA")
            .HasDefaultValueSql("SEQ_CLINICA.NEXTVAL")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(c => c.Cnpj)
            .HasColumnName("CNPJ")
            .HasMaxLength(14)
            .IsRequired();

        // A V11 do Flyway alargou a coluna de 120 para 150 e criou UNIQUE nela:
        // a autenticação do Java busca a clínica por e-mail, então dois
        // registros com o mesmo valor deixavam o login ambíguo.
        builder.Property(c => c.Email)
            .HasColumnName("EMAIL")
            .HasMaxLength(150);

        builder.Property(c => c.Telefone)
            .HasColumnName("TELEFONE")
            .HasMaxLength(20);

        builder.Property(c => c.Endereco)
            .HasColumnName("ENDERECO")
            .HasMaxLength(200);

        builder.Property(c => c.Ativo)
            .HasColumnName("ATIVO")
            .HasConversion(
                ativo => ativo ? "S" : "N",
                valor => valor == "S"
            )
            .HasMaxLength(1)
            .IsRequired();

        // Quem pode autenticar, criado pela V11. Só a API Java escreve neste
        // campo (é ela que aprova o credenciamento); o .NET lê para o
        // dashboard saber quais clínicas ainda estão em análise.
        builder.Property(c => c.StatusAcesso)
            .HasColumnName("STATUS_ACESSO")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(c => c.Cnpj)
            .IsUnique();
    }
}