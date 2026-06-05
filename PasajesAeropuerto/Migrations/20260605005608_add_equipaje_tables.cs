using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PasajesAeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class add_equipaje_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposEquipaje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEquipaje", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasajeroId = table.Column<int>(type: "int", nullable: false),
                    TipoEquipajeId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipajes", x => x.Id);
                    table.CheckConstraint("CK_Equipaje_Cantidad", "[Cantidad] > 0");
                    table.ForeignKey(
                        name: "FK_Equipajes_Pasajeros_PasajeroId",
                        column: x => x.PasajeroId,
                        principalTable: "Pasajeros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Equipajes_TiposEquipaje_TipoEquipajeId",
                        column: x => x.TipoEquipajeId,
                        principalTable: "TiposEquipaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TiposEquipaje",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Equipaje de mano" },
                    { 2, "Valija" },
                    { 3, "Equipaje adicional" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipajes_PasajeroId",
                table: "Equipajes",
                column: "PasajeroId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipajes_TipoEquipajeId",
                table: "Equipajes",
                column: "TipoEquipajeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipajes");

            migrationBuilder.DropTable(
                name: "TiposEquipaje");
        }
    }
}
