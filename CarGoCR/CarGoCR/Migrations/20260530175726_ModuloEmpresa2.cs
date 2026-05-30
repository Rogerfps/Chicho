using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarGoCR.Migrations
{
    /// <inheritdoc />
    public partial class ModuloEmpresa2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Industria",
                table: "Empresas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Industria",
                table: "Empresas");
        }
    }
}
