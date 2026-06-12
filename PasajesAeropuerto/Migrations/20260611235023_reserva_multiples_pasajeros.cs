using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasajesAeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class reserva_multiples_pasajeros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservaId",
                table: "Pasajeros",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE p
                SET p.ReservaId = r.Id
                FROM Pasajeros p
                INNER JOIN Reservas r ON r.PasajeroId = p.Id
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Pasajeros_PasajeroId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_PasajeroId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "PasajeroId",
                table: "Reservas");

            migrationBuilder.CreateIndex(
                name: "IX_Pasajeros_ReservaId",
                table: "Pasajeros",
                column: "ReservaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pasajeros_Reservas_ReservaId",
                table: "Pasajeros",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pasajeros_Reservas_ReservaId",
                table: "Pasajeros");

            migrationBuilder.DropIndex(
                name: "IX_Pasajeros_ReservaId",
                table: "Pasajeros");

            migrationBuilder.AddColumn<int>(
                name: "PasajeroId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(@"
                UPDATE r
                SET r.PasajeroId = p.Id
                FROM Reservas r
                INNER JOIN (
                    SELECT ReservaId, MIN(Id) AS PasajeroId
                    FROM Pasajeros
                    WHERE ReservaId IS NOT NULL
                    GROUP BY ReservaId
                ) p ON p.ReservaId = r.Id
            ");

            migrationBuilder.DropColumn(
                name: "ReservaId",
                table: "Pasajeros");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_PasajeroId",
                table: "Reservas",
                column: "PasajeroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Pasajeros_PasajeroId",
                table: "Reservas",
                column: "PasajeroId",
                principalTable: "Pasajeros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
