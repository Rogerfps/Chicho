using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarGoCR.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProformas2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proformas_Tarifas_TarifaId",
                table: "Proformas");

            migrationBuilder.DropIndex(
                name: "IX_Proformas_TarifaId",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "CostoEstimado",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "PesoEstimado",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "TarifaId",
                table: "Proformas");

            migrationBuilder.RenameColumn(
                name: "ValorDeclarado",
                table: "Proformas",
                newName: "Total");

            migrationBuilder.AddColumn<bool>(
                name: "Pagada",
                table: "Proformas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProformaDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProformaId = table.Column<int>(type: "integer", nullable: false),
                    PaqueteId = table.Column<int>(type: "integer", nullable: false),
                    Precio = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProformaDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProformaDetalle_Paquetes_PaqueteId",
                        column: x => x.PaqueteId,
                        principalTable: "Paquetes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProformaDetalle_Proformas_ProformaId",
                        column: x => x.ProformaId,
                        principalTable: "Proformas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProformaDetalle_PaqueteId",
                table: "ProformaDetalle",
                column: "PaqueteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaDetalle_ProformaId",
                table: "ProformaDetalle",
                column: "ProformaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProformaDetalle");

            migrationBuilder.DropColumn(
                name: "Pagada",
                table: "Proformas");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Proformas",
                newName: "ValorDeclarado");

            migrationBuilder.AddColumn<decimal>(
                name: "CostoEstimado",
                table: "Proformas",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Proformas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PesoEstimado",
                table: "Proformas",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TarifaId",
                table: "Proformas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proformas_TarifaId",
                table: "Proformas",
                column: "TarifaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proformas_Tarifas_TarifaId",
                table: "Proformas",
                column: "TarifaId",
                principalTable: "Tarifas",
                principalColumn: "Id");
        }
    }
}
