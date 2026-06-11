using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasajesAeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class add_reserva_fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Clase",
                table: "Reservas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Economica");

            migrationBuilder.AddColumn<int>(
                name: "DestinoId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "PasajeroId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "VueloId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_DestinoId",
                table: "Reservas",
                column: "DestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_PasajeroId",
                table: "Reservas",
                column: "PasajeroId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_VueloId",
                table: "Reservas",
                column: "VueloId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Destinos_DestinoId",
                table: "Reservas",
                column: "DestinoId",
                principalTable: "Destinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Pasajeros_PasajeroId",
                table: "Reservas",
                column: "PasajeroId",
                principalTable: "Pasajeros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Vuelos_VueloId",
                table: "Reservas",
                column: "VueloId",
                principalTable: "Vuelos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Destinos_DestinoId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Pasajeros_PasajeroId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Vuelos_VueloId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_DestinoId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_PasajeroId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_VueloId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Clase",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "DestinoId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "PasajeroId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "VueloId",
                table: "Reservas");
        }
    }
}
