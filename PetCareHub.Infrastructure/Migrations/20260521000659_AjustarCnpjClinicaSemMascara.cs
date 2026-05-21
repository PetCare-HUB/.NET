using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCareHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustarCnpjClinicaSemMascara : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NOME",
                table: "CLINICA",
                type: "NVARCHAR2(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "EMAIL",
                table: "CLINICA",
                type: "NVARCHAR2(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CNPJ",
                table: "CLINICA",
                type: "NVARCHAR2(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(18)",
                oldMaxLength: 18);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NOME",
                table: "CLINICA",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "EMAIL",
                table: "CLINICA",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CNPJ",
                table: "CLINICA",
                type: "NVARCHAR2(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(14)",
                oldMaxLength: 14);
        }
    }
}
