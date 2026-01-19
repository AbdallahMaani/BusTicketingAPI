using System.ComponentModel.DataAnnotations;

namespace Bus_ticketing_Backend.DTOs
{
    public class BookingDto
    {
        public Guid BookingId { get; set; } 
        public Guid UserId { get; set; } 
        public Guid TripId { get; set; } 
        public string OriginName { get; set; }
        public string DestinationName { get; set; }
        public string BusType { get; set; }
        public DateTime BookingDate { get; set; }
        public string? bookingStatus { get; set; }
        public decimal PriceTotal { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateBookingDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid TripId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public string bookingStatus { get; set; } = "Confirmed";

        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
        public int Quantity { get; set; }
    }

    public class UpdateBookingQuantityDto
    {
        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
        public int Quantity { get; set; }
    }

    public class UpdateBookingStatusDto
    {
        [Required]
        [RegularExpression("^(Confirmed|Cancelled)$", ErrorMessage = "Status must be either 'Confirmed' or 'Cancelled'")]
        public string Status { get; set; }
    }
}