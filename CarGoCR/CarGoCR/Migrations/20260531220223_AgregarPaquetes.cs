using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarGoCR.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPaquetes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Paquetes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tracking = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Peso = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorDeclarado = table.Column<decimal>(type: "numeric", nullable: false),
                    CostoEnvio = table.Column<decimal>(type: "numeric", nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Pagado = table.Column<bool>(type: "boolean", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    TarifaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paquetes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paquetes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Paquetes_Tarifas_TarifaId",
                        column: x => x.TarifaId,
                        principalTable: "Tarifas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_ClienteId",
                table: "Paquetes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_TarifaId",
                table: "Paquetes",
                column: "TarifaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Paquetes");
        }
    }
}
