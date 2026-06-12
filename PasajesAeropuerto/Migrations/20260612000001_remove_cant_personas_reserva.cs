using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasajesAeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class remove_cant_personas_reserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantPersonas",
                table: "Reservas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantPersonas",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
