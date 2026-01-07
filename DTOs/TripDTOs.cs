using System.ComponentModel.DataAnnotations;

namespace Bus_ticketing_Backend.DTOs
{
    public class TripDto
    {
        public Guid TripId { get; set; }
        public Guid RouteId { get; set; }
        public Guid BusId { get; set; }
        public DateOnly DepartureDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PriceJod { get; set; }
        public string Status { get; set; }

        // Route Info
        public string OriginName { get; set; }
        public string DestinationName { get; set; }

        // Bus Info
        public string BusType { get; set; }
        public string DriverName { get; set; }
        public string Features { get; set; }

        // ORIGIN STATION (Departure)
        public Guid OriginStationId { get; set; }
        public string OriginStationName { get; set; }
        public string OriginStationNameEn { get; set; }
        public string OriginStationStreet { get; set; }
        public double? OriginStationLat { get; set; }
        public double? OriginStationLng { get; set; }

        // DESTINATION STATION (Arrival)
        public Guid DestinationStationId { get; set; }
        public string DestinationStationName { get; set; }
        public string DestinationStationNameEn { get; set; }
        public string DestinationStationStreet { get; set; }
        public double? DestinationStationLat { get; set; }
        public double? DestinationStationLng { get; set; }
    }

    public class CreateTripDto
    {
        [Required]
        public Guid RouteId { get; set; }

        [Required]
        public Guid BusId { get; set; }

        [Required]
        public Guid OriginStationId { get; set; }

        [Required]
        public Guid DestinationStationId { get; set; }

        [Required]
        public DateOnly DepartureDate { get; set; }

        [Required]
        public TimeOnly DepartureTime { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        [Required]
        public decimal PriceJod { get; set; }

        public string Status { get; set; } = "Scheduled";
    }

    public class UpdateTripDto
    {
        public Guid RouteId { get; set; }
        public Guid BusId { get; set; }
        public Guid OriginStationId { get; set; }
        public Guid DestinationStationId { get; set; }
        public DateOnly DepartureDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PriceJod { get; set; }
        public string Status { get; set; }
    }
}