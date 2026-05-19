using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCareHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLINICA",
                columns: table => new
                {
                    ID_CLINICA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CNPJ = table.Column<string>(type: "NVARCHAR2(18)", maxLength: 18, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    ATIVO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLINICA", x => x.ID_CLINICA);
                });

            migrationBuilder.CreateTable(
                name: "PET",
                columns: table => new
                {
                    ID_PET = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_RESPONSAVEL = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_CLINICA = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESPECIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    RACA = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true),
                    DATA_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PESO_KG = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    SEXO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: true),
                    CONDICOES_CRONICAS = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    DATA_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ATIVO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PET", x => x.ID_PET);
                    table.ForeignKey(
                        name: "FK_PET_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONSULTA",
                columns: table => new
                {
                    ID_CONSULTA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PET = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_CLINICA = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    DATA_CONSULTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TIPO_CONSULTA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    DIAGNOSTICO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    VALOR = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    RETORNO_RECOMENDADO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false),
                    DATA_RETORNO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONSULTA", x => x.ID_CONSULTA);
                    table.ForeignKey(
                        name: "FK_CONSULTA_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONSULTA_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EVENTO_PREVENTIVO",
                columns: table => new
                {
                    ID_EVENTO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PET = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_PROTOCOLO = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    TIPO_EVENTO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    DATA_PREVISTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATA_REALIZACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTO_PREVENTIVO", x => x.ID_EVENTO);
                    table.ForeignKey(
                        name: "FK_EVENTO_PREVENTIVO_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LEITURA_SENSOR",
                columns: table => new
                {
                    ID_LEITURA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PET = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_DISPOSITIVO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TIPO_LEITURA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    VALOR = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    UNIDADE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DATA_LEITURA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    STATUS_LEITURA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEITURA_SENSOR", x => x.ID_LEITURA);
                    table.ForeignKey(
                        name: "FK_LEITURA_SENSOR_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SCORE_SAUDE",
                columns: table => new
                {
                    ID_SCORE = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PET = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    SCORE_TOTAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SCORE_ATIVIDADE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SCORE_ALIMENTACAO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SCORE_AMBIENTE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SCORE_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SCORE_PREVENTIVO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CATEGORIA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DATA_CALCULO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCORE_SAUDE", x => x.ID_SCORE);
                    table.ForeignKey(
                        name: "FK_SCORE_SAUDE_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ALERTA_SAUDE",
                columns: table => new
                {
                    ID_ALERTA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PET = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_LEITURA = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    TIPO_ALERTA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NIVEL_ALERTA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    MENSAGEM = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    VALOR_DETECTADO = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    LIMITE_REFERENCIA = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    RESOLVIDO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false),
                    DATA_ALERTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATA_RESOLUCAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALERTA_SAUDE", x => x.ID_ALERTA);
                    table.ForeignKey(
                        name: "FK_ALERTA_SAUDE_LEITURA_SENSOR_ID_LEITURA",
                        column: x => x.ID_LEITURA,
                        principalTable: "LEITURA_SENSOR",
                        principalColumn: "ID_LEITURA",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ALERTA_SAUDE_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_SAUDE_DATA_ALERTA",
                table: "ALERTA_SAUDE",
                column: "DATA_ALERTA");

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_SAUDE_ID_LEITURA",
                table: "ALERTA_SAUDE",
                column: "ID_LEITURA");

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_SAUDE_ID_PET",
                table: "ALERTA_SAUDE",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_SAUDE_NIVEL_ALERTA",
                table: "ALERTA_SAUDE",
                column: "NIVEL_ALERTA");

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_SAUDE_RESOLVIDO",
                table: "ALERTA_SAUDE",
                column: "RESOLVIDO");

            migrationBuilder.CreateIndex(
                name: "IX_CLINICA_CNPJ",
                table: "CLINICA",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_DATA_CONSULTA",
                table: "CONSULTA",
                column: "DATA_CONSULTA");

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_ID_CLINICA",
                table: "CONSULTA",
                column: "ID_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_ID_PET",
                table: "CONSULTA",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_PREVENTIVO_DATA_PREVISTA",
                table: "EVENTO_PREVENTIVO",
                column: "DATA_PREVISTA");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_PREVENTIVO_ID_PET",
                table: "EVENTO_PREVENTIVO",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_PREVENTIVO_STATUS",
                table: "EVENTO_PREVENTIVO",
                column: "STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_LEITURA_SENSOR_DATA_LEITURA",
                table: "LEITURA_SENSOR",
                column: "DATA_LEITURA");

            migrationBuilder.CreateIndex(
                name: "IX_LEITURA_SENSOR_ID_PET",
                table: "LEITURA_SENSOR",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_LEITURA_SENSOR_TIPO_LEITURA",
                table: "LEITURA_SENSOR",
                column: "TIPO_LEITURA");

            migrationBuilder.CreateIndex(
                name: "IX_PET_ESPECIE",
                table: "PET",
                column: "ESPECIE");

            migrationBuilder.CreateIndex(
                name: "IX_PET_ID_CLINICA",
                table: "PET",
                column: "ID_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_PET_ID_RESPONSAVEL",
                table: "PET",
                column: "ID_RESPONSAVEL");

            migrationBuilder.CreateIndex(
                name: "IX_SCORE_SAUDE_CATEGORIA",
                table: "SCORE_SAUDE",
                column: "CATEGORIA");

            migrationBuilder.CreateIndex(
                name: "IX_SCORE_SAUDE_DATA_CALCULO",
                table: "SCORE_SAUDE",
                column: "DATA_CALCULO");

            migrationBuilder.CreateIndex(
                name: "IX_SCORE_SAUDE_ID_PET",
                table: "SCORE_SAUDE",
                column: "ID_PET");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALERTA_SAUDE");

            migrationBuilder.DropTable(
                name: "CONSULTA");

            migrationBuilder.DropTable(
                name: "EVENTO_PREVENTIVO");

            migrationBuilder.DropTable(
                name: "SCORE_SAUDE");

            migrationBuilder.DropTable(
                name: "LEITURA_SENSOR");

            migrationBuilder.DropTable(
                name: "PET");

            migrationBuilder.DropTable(
                name: "CLINICA");
        }
    }
}
