using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bus_ticketing_Backend.Migrations
{
    /// <inheritdoc />
    public partial class fixingcreatebooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "tripStatus",
                table: "Trips",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bookingStatus",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "tripStatus",
                value: "Scheduled");

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "tripStatus",
                value: "Scheduled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tripStatus",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "bookingStatus",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Trips",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Status",
                value: "Scheduled");

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Status",
                value: "Scheduled");
        }
    }
}
