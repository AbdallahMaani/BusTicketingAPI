using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Bus_ticketingAPI.Entities;

namespace Bus_ticketing_Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } 
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Always call base first to ensure default behaviors are set
            base.OnModelCreating(modelBuilder);

            // 1. Converters for DateOnly / TimeOnly 
            // We define these as static to ensure the comparison doesn't fail during migration checks
            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                dt => DateOnly.FromDateTime(dt));

            var timeOnlyConverter = new ValueConverter<TimeOnly, TimeSpan>(
                t => t.ToTimeSpan(),
                ts => TimeOnly.FromTimeSpan(ts));

            // 2. Entity Mappings
            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.TripId);
                entity.Property(e => e.PriceJod).HasColumnType("decimal(18,2)");
                
                // Set converters explicitly
                entity.Property(e => e.DepartureDate)
                      .HasConversion(dateOnlyConverter)
                      .HasColumnType("date");

                entity.Property(e => e.DepartureTime)
                      .HasConversion(timeOnlyConverter)
                      .HasColumnType("time");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingId);
                entity.Property(e => e.PriceTotal).HasColumnType("decimal(18,2)");
                // Ensure DateTime is handled correctly for PostgreSQL
                entity.Property(e => e.BookingDate).HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<City>().HasData(
                new City { Id = "LOC_AMN", NameEn = "Amman", NameAr = "عمان" },
                new City { Id = "LOC_IRB", NameEn = "Irbid", NameAr = "إربد" },
                new City { Id = "LOC_ZAR", NameEn = "Zarqa", NameAr = "الزرقاء" },
                new City { Id = "LOC_BAL", NameEn = "Balqa", NameAr = "البلقاء" },
                new City { Id = "LOC_MAD", NameEn = "Madaba", NameAr = "مأدبا" },
                new City { Id = "LOC_KAR", NameEn = "Karak", NameAr = "الكرك" },
                new City { Id = "LOC_TAF", NameEn = "Tafilah", NameAr = "الطفيلة" },
                new City { Id = "LOC_MAA", NameEn = "Ma'an", NameAr = "معان" },
                new City { Id = "LOC_AQA", NameEn = "Aqaba", NameAr = "العقبة" },
                new City { Id = "LOC_AJL", NameEn = "Ajloun", NameAr = "عجلون" },
                new City { Id = "LOC_JER", NameEn = "Jerash", NameAr = "جرش" },
                new City { Id = "LOC_MAF", NameEn = "Mafraq", NameAr = "المفرق" }
            );

                var station1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
                var station2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

                var bus1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
                var bus2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

                var route1Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
                var route2Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

                var trip1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
                var trip2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");


            modelBuilder.Entity<Station>().HasData(
                new Station { Id = station1Id, CityId = "LOC_AMN", NameEn = "Tabarbour", StationName = "North Terminal", StreetEn = "Tareq St", Lat = 31.99, Lng = 35.91 },
                new Station { Id = station2Id, CityId = "LOC_IRB", NameEn = "Irbid City Center", StationName = "Amman Bus Station", StreetEn = "University St", Lat = 32.55, Lng = 35.85 }
            );

            // Seed Buses
            modelBuilder.Entity<Bus>().HasData(
                new Bus { BusId = bus1Id, Operator = "JET", Type = "VIP", Capacity = 45, Model = "Mercedes-Benz Travego", ModelYear = 2023, DriverName = "Sami Ahmad", Features = "WiFi,AC,USB Charger" },
                new Bus { BusId = bus2Id, Operator = "Hijazi", Type = "Standard", Capacity = 50, Model = "Volvo B11R", ModelYear = 2021, DriverName = "Omar Khaled", Features = "WiFi,AC,USB Charger,W/C" }
            );

            modelBuilder.Entity<Routes>().HasData(
                new Routes { RouteId = route1Id, OriginId = "LOC_AMN", DestinationId = "LOC_IRB", DistanceKm = 80, DurationHrs = 1.5 },
                new Routes { RouteId = route2Id, OriginId = "LOC_IRB", DestinationId = "LOC_AMN", DistanceKm = 80, DurationHrs = 1.5 }
            );

            modelBuilder.Entity<Trip>().HasData(
                new Trip { TripId = trip1Id, RouteId = route1Id, BusId = bus1Id, DepartureDate = new DateOnly(2025, 12, 30), DepartureTime = new TimeOnly(08, 30), AvailableSeats = 45, PriceJod = 2.50m, tripStatus = "Scheduled" },
                new Trip { TripId = trip2Id, RouteId = route2Id, BusId = bus2Id, DepartureDate = new DateOnly(2025, 12, 30), DepartureTime = new TimeOnly(14, 00), AvailableSeats = 50, PriceJod = 2.00m, tripStatus = "Scheduled" }
            );

            
        }
    }
}