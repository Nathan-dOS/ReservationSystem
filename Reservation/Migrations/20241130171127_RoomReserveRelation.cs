using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservation.Migrations
{
    /// <inheritdoc />
    public partial class RoomReserveRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reserves_RoomId",
                table: "Reserves",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Rooms_RoomId",
                table: "Reserves",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_Rooms_RoomId",
                table: "Reserves");

            migrationBuilder.DropIndex(
                name: "IX_Reserves_RoomId",
                table: "Reserves");
        }
    }
}
