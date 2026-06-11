using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PasajesAeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class add_origen_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Origenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Origenes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Origenes",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Buenos Aires" },
                    { 2, "Córdoba" },
                    { 3, "Mendoza" }
                });

            migrationBuilder.AddColumn<int>(
                name: "OrigenId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_OrigenId",
                table: "Reservas",
                column: "OrigenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Origenes_OrigenId",
                table: "Reservas",
                column: "OrigenId",
                principalTable: "Origenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Origenes_OrigenId",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "Origenes");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_OrigenId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "OrigenId",
                table: "Reservas");
        }
    }
}
