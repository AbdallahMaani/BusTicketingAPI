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
        public string OriginName { get; set; }     
        public string DestinationName { get; set; } 
        public string BusType { get; set; }
    }

    public class CreateTripDto
    {
        [Required]
        public Guid RouteId { get; set; }

        [Required]
        public Guid BusId { get; set; }

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
        public DateOnly DepartureDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PriceJod { get; set; }
        public string Status { get; set; }
    }
}