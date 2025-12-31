using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bus_ticketing_Backend.Migrations
{
    /// <inheritdoc />
    public partial class intial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buses",
                columns: table => new
                {
                    BusId = table.Column<Guid>(type: "uuid", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    ModelYear = table.Column<int>(type: "integer", nullable: false),
                    DriverName = table.Column<string>(type: "text", nullable: false),
                    Features = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buses", x => x.BusId);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginId = table.Column<string>(type: "text", nullable: false),
                    DestinationId = table.Column<string>(type: "text", nullable: false),
                    DistanceKm = table.Column<int>(type: "integer", nullable: false),
                    DurationHrs = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteId);
                    table.ForeignKey(
                        name: "FK_Routes_Cities_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Routes_Cities_OriginId",
                        column: x => x.OriginId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CityId = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    StationName = table.Column<string>(type: "text", nullable: false),
                    StreetEn = table.Column<string>(type: "text", nullable: false),
                    Lat = table.Column<double>(type: "double precision", nullable: true),
                    Lng = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stations_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    TripId = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "date", nullable: false),
                    DepartureTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AvailableSeats = table.Column<int>(type: "integer", nullable: false),
                    PriceJod = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.TripId);
                    table.ForeignKey(
                        name: "FK_Trips_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trips_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TripId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PricePaid = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Buses",
                columns: new[] { "BusId", "Capacity", "DriverName", "Features", "Model", "ModelYear", "Operator", "Type" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 45, "Sami Ahmad", "WiFi,AC,USB Charger", "Mercedes-Benz Travego", 2023, "JET", "VIP" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), 50, "Omar Khaled", "WiFi,AC,USB Charger,W/C", "Volvo B11R", 2021, "Hijazi", "Standard" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { "LOC_AJL", "عجلون", "Ajloun" },
                    { "LOC_AMN", "عمان", "Amman" },
                    { "LOC_AQA", "العقبة", "Aqaba" },
                    { "LOC_BAL", "البلقاء", "Balqa" },
                    { "LOC_IRB", "إربد", "Irbid" },
                    { "LOC_JER", "جرش", "Jerash" },
                    { "LOC_KAR", "الكرك", "Karak" },
                    { "LOC_MAA", "معان", "Ma'an" },
                    { "LOC_MAD", "مأدبا", "Madaba" },
                    { "LOC_MAF", "المفرق", "Mafraq" },
                    { "LOC_TAF", "الطفيلة", "Tafilah" },
                    { "LOC_ZAR", "الزرقاء", "Zarqa" }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "RouteId", "DestinationId", "DistanceKm", "DurationHrs", "OriginId" },
                values: new object[,]
                {
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "LOC_IRB", 80, 1.5, "LOC_AMN" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "LOC_AMN", 80, 1.5, "LOC_IRB" }
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "CityId", "Lat", "Lng", "NameEn", "StationName", "StreetEn" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "LOC_AMN", 31.989999999999998, 35.909999999999997, "Tabarbour", "North Terminal", "Tareq St" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "LOC_IRB", 32.549999999999997, 35.850000000000001, "Irbid City Center", "Amman Bus Station", "University St" }
                });

            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "TripId", "AvailableSeats", "BusId", "DepartureDate", "DepartureTime", "PriceJod", "RouteId", "Status" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 45, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2025, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 30, 0, 0), 2.50m, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Scheduled" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 50, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2025, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 14, 0, 0, 0), 2.00m, new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "Scheduled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TripId",
                table: "Bookings",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationId",
                table: "Routes",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginId",
                table: "Routes",
                column: "OriginId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_CityId",
                table: "Stations",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_BusId",
                table: "Trips",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_RouteId",
                table: "Trips",
                column: "RouteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Buses");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
