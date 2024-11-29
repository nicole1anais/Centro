using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Centro.Migrations
{
    /// <inheritdoc />
    public partial class secundaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitud",
                table: "Profesionales",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitud",
                table: "Profesionales",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "Profesionales");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "Profesionales");
        }
    }
}
