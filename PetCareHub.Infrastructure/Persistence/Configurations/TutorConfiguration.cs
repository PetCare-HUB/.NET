using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Configurations;

public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("TUTOR");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("ID_TUTOR")
            .HasDefaultValueSql("SEQ_TUTOR.NEXTVAL")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Email)
            .HasColumnName("EMAIL")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(t => t.Telefone)
            .HasColumnName("TELEFONE")
            .HasMaxLength(20);

        builder.Property(t => t.Cpf)
            .HasColumnName("CPF")
            .HasMaxLength(11);

        builder.Property(t => t.DataCadastro)
            .HasColumnName("DATA_CADASTRO")
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(t => t.StatusAcesso)
            .HasColumnName("STATUS_ACESSO")
            .HasMaxLength(20)
            .IsRequired();

        // Credencial (SENHA_HASH) e a coluna legada ATIVO não são mapeadas de propósito:
        // autenticação é responsabilidade exclusiva do Java, o dashboard .NET só lê o tutor.

        builder.HasIndex(t => t.Email).IsUnique();
        builder.HasIndex(t => t.Cpf).IsUnique();
    }
}
