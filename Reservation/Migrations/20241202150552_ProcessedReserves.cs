using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservation.Migrations
{
    /// <inheritdoc />
    public partial class ProcessedReserves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "Reserves",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Processed",
                table: "Reserves");
        }
    }
}
