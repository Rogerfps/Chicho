using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarGoCR.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProformaDetalles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProformaDetalle_Paquetes_PaqueteId",
                table: "ProformaDetalle");

            migrationBuilder.DropForeignKey(
                name: "FK_ProformaDetalle_Proformas_ProformaId",
                table: "ProformaDetalle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProformaDetalle",
                table: "ProformaDetalle");

            migrationBuilder.RenameTable(
                name: "ProformaDetalle",
                newName: "ProformaDetalles");

            migrationBuilder.RenameIndex(
                name: "IX_ProformaDetalle_ProformaId",
                table: "ProformaDetalles",
                newName: "IX_ProformaDetalles_ProformaId");

            migrationBuilder.RenameIndex(
                name: "IX_ProformaDetalle_PaqueteId",
                table: "ProformaDetalles",
                newName: "IX_ProformaDetalles_PaqueteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProformaDetalles",
                table: "ProformaDetalles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProformaDetalles_Paquetes_PaqueteId",
                table: "ProformaDetalles",
                column: "PaqueteId",
                principalTable: "Paquetes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProformaDetalles_Proformas_ProformaId",
                table: "ProformaDetalles",
                column: "ProformaId",
                principalTable: "Proformas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProformaDetalles_Paquetes_PaqueteId",
                table: "ProformaDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_ProformaDetalles_Proformas_ProformaId",
                table: "ProformaDetalles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProformaDetalles",
                table: "ProformaDetalles");

            migrationBuilder.RenameTable(
                name: "ProformaDetalles",
                newName: "ProformaDetalle");

            migrationBuilder.RenameIndex(
                name: "IX_ProformaDetalles_ProformaId",
                table: "ProformaDetalle",
                newName: "IX_ProformaDetalle_ProformaId");

            migrationBuilder.RenameIndex(
                name: "IX_ProformaDetalles_PaqueteId",
                table: "ProformaDetalle",
                newName: "IX_ProformaDetalle_PaqueteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProformaDetalle",
                table: "ProformaDetalle",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProformaDetalle_Paquetes_PaqueteId",
                table: "ProformaDetalle",
                column: "PaqueteId",
                principalTable: "Paquetes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProformaDetalle_Proformas_ProformaId",
                table: "ProformaDetalle",
                column: "ProformaId",
                principalTable: "Proformas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
