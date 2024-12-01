using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservation.Migrations
{
    /// <inheritdoc />
    public partial class NewAttributesToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "RentPrice",
                table: "ReserveHistories",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "IX_ReserveHistories_RoomId",
                table: "ReserveHistories",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveHistories_Rooms_RoomId",
                table: "ReserveHistories",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReserveHistories_Rooms_RoomId",
                table: "ReserveHistories");

            migrationBuilder.DropIndex(
                name: "IX_ReserveHistories_RoomId",
                table: "ReserveHistories");

            migrationBuilder.DropColumn(
                name: "RentPrice",
                table: "ReserveHistories");
        }
    }
}
