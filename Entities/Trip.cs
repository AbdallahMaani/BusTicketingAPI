using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_ticketingAPI.Entities
{
    public class Trip
    {
        [Key]
        public Guid TripId { get; set; } = Guid.NewGuid();
        [Required]
        public Guid RouteId { get; set; }
        [ForeignKey("RouteId")]
        public Routes? Route { get; set; }
        [Required]
        public Guid BusId { get; set; }
        [ForeignKey("BusId")]
        public Bus? Bus { get; set; }
        public DateOnly DepartureDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public int AvailableSeats { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceJod { get; set; }
        public string? tripStatus { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}