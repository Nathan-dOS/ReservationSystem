using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservation.Migrations
{
    /// <inheritdoc />
    public partial class ReserveEquipments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipementId",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "EquipmentStatus",
                table: "Equipments");

            migrationBuilder.CreateTable(
                name: "ReserveEquipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReserveId = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReserveEquipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReserveEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReserveEquipments_Reserves_ReserveId",
                        column: x => x.ReserveId,
                        principalTable: "Reserves",
                        principalColumn: "ReserveId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReserveEquipments_EquipmentId",
                table: "ReserveEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveEquipments_ReserveId",
                table: "ReserveEquipments",
                column: "ReserveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReserveEquipments");

            migrationBuilder.AddColumn<int>(
                name: "EquipementId",
                table: "Reserves",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentStatus",
                table: "Equipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
