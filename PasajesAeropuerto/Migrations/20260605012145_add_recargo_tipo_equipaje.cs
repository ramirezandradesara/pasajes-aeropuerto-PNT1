using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasajesAeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class add_recargo_tipo_equipaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Recargo",
                table: "TiposEquipaje",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "TiposEquipaje",
                keyColumn: "Id",
                keyValue: 1,
                column: "Recargo",
                value: 0.00m);

            migrationBuilder.UpdateData(
                table: "TiposEquipaje",
                keyColumn: "Id",
                keyValue: 2,
                column: "Recargo",
                value: 8000.00m);

            migrationBuilder.UpdateData(
                table: "TiposEquipaje",
                keyColumn: "Id",
                keyValue: 3,
                column: "Recargo",
                value: 12000.00m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recargo",
                table: "TiposEquipaje");
        }
    }
}
