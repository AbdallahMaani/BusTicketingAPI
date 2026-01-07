using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bus_ticketing_Backend.Migrations
{
    /// <inheritdoc />
    public partial class addingStationToTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StationId",
                table: "Trips",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "StationId",
                value: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "StationId",
                value: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StationId",
                table: "Trips",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Stations_StationId",
                table: "Trips",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Stations_StationId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_StationId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "Trips");
        }
    }
}
