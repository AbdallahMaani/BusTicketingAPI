using System.ComponentModel.DataAnnotations;

namespace Bus_ticketing_Backend.DTOs
{
    public class BookingDto
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; } 
        public Guid TripId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public decimal PricePaid { get; set; }
    }

    public class CreateBookingDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid TripId { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public decimal PricePaid { get; set; }
        public string Status { get; set; } = "Confirmed";
    }

    public class UpdateBookingDto
    {
        public string Status { get; set; }
    }
}