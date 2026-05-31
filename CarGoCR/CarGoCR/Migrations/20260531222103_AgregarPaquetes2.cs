using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarGoCR.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPaquetes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paquetes_Tarifas_TarifaId",
                table: "Paquetes");

            migrationBuilder.AlterColumn<int>(
                name: "TarifaId",
                table: "Paquetes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Paquetes_Tarifas_TarifaId",
                table: "Paquetes",
                column: "TarifaId",
                principalTable: "Tarifas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paquetes_Tarifas_TarifaId",
                table: "Paquetes");

            migrationBuilder.AlterColumn<int>(
                name: "TarifaId",
                table: "Paquetes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Paquetes_Tarifas_TarifaId",
                table: "Paquetes",
                column: "TarifaId",
                principalTable: "Tarifas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
