using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PasajesAeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class remove_aviones_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aviones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aviones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    Matricula = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aviones", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Aviones",
                columns: new[] { "Id", "Capacidad", "Matricula", "Modelo" },
                values: new object[,]
                {
                    { 1, 160, "LV-CKS", "Boeing 737" },
                    { 2, 150, "LV-GUR", "Airbus A320" },
                    { 3, 96, "LV-CHO", "Embraer 190" }
                });
        }
    }
}
