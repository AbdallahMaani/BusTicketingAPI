using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bus_ticketing_Backend.Migrations
{
    /// <inheritdoc />
    public partial class GettingOriginAndDestFromTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Stations_StationId",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "Trips",
                newName: "OriginStationId");

            migrationBuilder.RenameIndex(
                name: "IX_Trips_StationId",
                table: "Trips",
                newName: "IX_Trips_OriginStationId");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationStationId",
                table: "Trips",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "DestinationStationId",
                value: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "DestinationStationId",
                value: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DestinationStationId",
                table: "Trips",
                column: "DestinationStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Stations_DestinationStationId",
                table: "Trips",
                column: "DestinationStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Stations_OriginStationId",
                table: "Trips",
                column: "OriginStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Stations_DestinationStationId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Stations_OriginStationId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_DestinationStationId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DestinationStationId",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "OriginStationId",
                table: "Trips",
                newName: "StationId");

            migrationBuilder.RenameIndex(
                name: "IX_Trips_OriginStationId",
                table: "Trips",
                newName: "IX_Trips_StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Stations_StationId",
                table: "Trips",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
