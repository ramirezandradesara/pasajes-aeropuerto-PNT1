using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PasajesAeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class addseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Aviones",
                columns: new[] { "Id", "Capacidad", "Matricula", "Modelo" },
                values: new object[,]
                {
                    { 1, 160, "LV-CKS", "Boeing 737" },
                    { 2, 150, "LV-GUR", "Airbus A320" },
                    { 3, 96, "LV-CHO", "Embraer 190" }
                });

            migrationBuilder.InsertData(
                table: "Destinos",
                columns: new[] { "Id", "Km", "Nombre", "PrecioBase" },
                values: new object[,]
                {
                    { 1, 0.0, "Buenos Aires", 0.00m },
                    { 2, 1350.5, "Bariloche", 45000.00m },
                    { 3, 1050.2, "Mendoza", 32000.00m },
                    { 4, 700.0, "Córdoba", 22000.00m }
                });

            migrationBuilder.InsertData(
                table: "Vuelos",
                columns: new[] { "Id", "Aerolinea", "EsTemporadaAlta", "FechaSalida", "HoraLlegada", "HoraSalida", "Numero" },
                values: new object[,]
                {
                    { 1, "Aerolíneas Argentinas", false, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 10, 45, 0, 0), new TimeSpan(0, 8, 30, 0, 0), "AR1400" },
                    { 2, "JetSMART", true, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 16, 30, 0, 0), new TimeSpan(0, 14, 15, 0, 0), "WJ3420" },
                    { 3, "Flybondi", false, new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 20, 30, 0, 0), new TimeSpan(0, 19, 0, 0, 0), "FB5200" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Aviones",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Aviones",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Aviones",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Destinos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Destinos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Destinos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Destinos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vuelos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vuelos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vuelos",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
