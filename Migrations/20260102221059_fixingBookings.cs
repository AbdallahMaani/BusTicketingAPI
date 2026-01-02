using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bus_ticketing_Backend.Migrations
{
    /// <inheritdoc />
    public partial class fixingBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Buses_BusId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BusId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BusId",
                table: "Bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BusId",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BusId",
                table: "Bookings",
                column: "BusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Buses_BusId",
                table: "Bookings",
                column: "BusId",
                principalTable: "Buses",
                principalColumn: "BusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
