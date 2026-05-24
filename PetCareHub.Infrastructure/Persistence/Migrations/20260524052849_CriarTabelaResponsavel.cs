using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCareHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaResponsavel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PESO_KG",
                table: "PET",
                type: "DECIMAL(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "NOME",
                table: "PET",
                type: "NVARCHAR2(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ESPECIE",
                table: "PET",
                type: "NVARCHAR2(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_NASCIMENTO",
                table: "PET",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_CADASTRO",
                table: "PET",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<string>(
                name: "CONDICOES_CRONICAS",
                table: "PET",
                type: "NVARCHAR2(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TIPO_LEITURA",
                table: "LEITURA_SENSOR",
                type: "NVARCHAR2(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "TIPO_CONSULTA",
                table: "CONSULTA",
                type: "NVARCHAR2(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_RETORNO",
                table: "CONSULTA",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_CONSULTA",
                table: "CONSULTA",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<string>(
                name: "TIPO_ALERTA",
                table: "ALERTA_SAUDE",
                type: "NVARCHAR2(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MENSAGEM",
                table: "ALERTA_SAUDE",
                type: "NVARCHAR2(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "RESPONSAVEL",
                columns: table => new
                {
                    ID_RESPONSAVEL = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    CPF = table.Column<string>(type: "NVARCHAR2(11)", maxLength: 11, nullable: true),
                    DATA_CADASTRO = table.Column<DateTime>(type: "DATE", nullable: false),
                    ATIVO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESPONSAVEL", x => x.ID_RESPONSAVEL);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RESPONSAVEL_CPF",
                table: "RESPONSAVEL",
                column: "CPF",
                unique: true,
                filter: "\"CPF\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RESPONSAVEL_EMAIL",
                table: "RESPONSAVEL",
                column: "EMAIL",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PET_RESPONSAVEL_ID_RESPONSAVEL",
                table: "PET",
                column: "ID_RESPONSAVEL",
                principalTable: "RESPONSAVEL",
                principalColumn: "ID_RESPONSAVEL",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PET_RESPONSAVEL_ID_RESPONSAVEL",
                table: "PET");

            migrationBuilder.DropTable(
                name: "RESPONSAVEL");

            migrationBuilder.AlterColumn<decimal>(
                name: "PESO_KG",
                table: "PET",
                type: "DECIMAL(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "NOME",
                table: "PET",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ESPECIE",
                table: "PET",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_NASCIMENTO",
                table: "PET",
                type: "TIMESTAMP(7)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_CADASTRO",
                table: "PET",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<string>(
                name: "CONDICOES_CRONICAS",
                table: "PET",
                type: "NVARCHAR2(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TIPO_LEITURA",
                table: "LEITURA_SENSOR",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "TIPO_CONSULTA",
                table: "CONSULTA",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_RETORNO",
                table: "CONSULTA",
                type: "TIMESTAMP(7)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_CONSULTA",
                table: "CONSULTA",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<string>(
                name: "TIPO_ALERTA",
                table: "ALERTA_SAUDE",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "MENSAGEM",
                table: "ALERTA_SAUDE",
                type: "NVARCHAR2(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(300)",
                oldMaxLength: 300);
        }
    }
}
