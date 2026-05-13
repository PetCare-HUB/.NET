using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCareHub.Infrastructure.Persistence.Migrations
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
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TUTOR_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CLINICA_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESPECIE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    RACA = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    SEXO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false),
                    DATA_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PESO_KG = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    CONDICOES_CRONICAS = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    ATIVO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PET", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PET_CLINICA_CLINICA_ID",
                        column: x => x.CLINICA_ID,
                        principalTable: "CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONSULTA",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PET_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CLINICA_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    DATA_CONSULTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TIPO_CONSULTA = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    OBSERVACOES = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    VALOR = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    RETORNO_RECOMENDADO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONSULTA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CONSULTA_CLINICA_CLINICA_ID",
                        column: x => x.CLINICA_ID,
                        principalTable: "CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONSULTA_PET_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "PET",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EVENTO_PREVENTIVO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PET_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TIPO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    DATA_PREVISTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATA_REALIZACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTO_PREVENTIVO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EVENTO_PREVENTIVO_PET_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "PET",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LEITURA_SENSOR",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PET_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TIPO_SENSOR = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    VALOR = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    UNIDADE_MEDIDA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    STATUS_LEITURA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DATA_LEITURA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEITURA_SENSOR", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LEITURA_SENSOR_PET_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "PET",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SCORE_SAUDE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PET_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    SCORE_TOTAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CATEGORIA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DATA_CALCULO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    OBSERVACAO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCORE_SAUDE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SCORE_SAUDE_PET_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "PET",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ALERTA_SAUDE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PET_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    LEITURA_SENSOR_ID = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    TIPO_ALERTA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    NIVEL = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    RESOLVIDO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATA_ALERTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATA_RESOLUCAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALERTA_SAUDE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ALERTA_SAUDE_LEITURA_SENSOR_LEITURA_SENSOR_ID",
                        column: x => x.LEITURA_SENSOR_ID,
                        principalTable: "LEITURA_SENSOR",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ALERTA_SAUDE_PET_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "PET",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_SAUDE_DATA_ALERTA",
                table: "ALERTA_SAUDE",
                column: "DATA_ALERTA");

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_SAUDE_LEITURA_SENSOR_ID",
                table: "ALERTA_SAUDE",
                column: "LEITURA_SENSOR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_SAUDE_NIVEL",
                table: "ALERTA_SAUDE",
                column: "NIVEL");

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_SAUDE_PET_ID",
                table: "ALERTA_SAUDE",
                column: "PET_ID");

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
                name: "IX_CONSULTA_CLINICA_ID",
                table: "CONSULTA",
                column: "CLINICA_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_DATA_CONSULTA",
                table: "CONSULTA",
                column: "DATA_CONSULTA");

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_PET_ID",
                table: "CONSULTA",
                column: "PET_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_PREVENTIVO_DATA_PREVISTA",
                table: "EVENTO_PREVENTIVO",
                column: "DATA_PREVISTA");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_PREVENTIVO_PET_ID",
                table: "EVENTO_PREVENTIVO",
                column: "PET_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_PREVENTIVO_STATUS",
                table: "EVENTO_PREVENTIVO",
                column: "STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_LEITURA_SENSOR_DATA_LEITURA",
                table: "LEITURA_SENSOR",
                column: "DATA_LEITURA");

            migrationBuilder.CreateIndex(
                name: "IX_LEITURA_SENSOR_PET_ID",
                table: "LEITURA_SENSOR",
                column: "PET_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEITURA_SENSOR_TIPO_SENSOR",
                table: "LEITURA_SENSOR",
                column: "TIPO_SENSOR");

            migrationBuilder.CreateIndex(
                name: "IX_PET_CLINICA_ID",
                table: "PET",
                column: "CLINICA_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PET_ESPECIE",
                table: "PET",
                column: "ESPECIE");

            migrationBuilder.CreateIndex(
                name: "IX_PET_TUTOR_ID",
                table: "PET",
                column: "TUTOR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SCORE_SAUDE_CATEGORIA",
                table: "SCORE_SAUDE",
                column: "CATEGORIA");

            migrationBuilder.CreateIndex(
                name: "IX_SCORE_SAUDE_DATA_CALCULO",
                table: "SCORE_SAUDE",
                column: "DATA_CALCULO");

            migrationBuilder.CreateIndex(
                name: "IX_SCORE_SAUDE_PET_ID",
                table: "SCORE_SAUDE",
                column: "PET_ID");
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
