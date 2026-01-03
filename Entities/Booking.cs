using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_ticketingAPI.Entities
{
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("TripId")]
        public Guid TripId { get; set; }
        public Trip Trip { get; set; }

        public DateTime BookingDate { get; set; }
        public string? bookingStatus { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceTotal { get; set; }
        public int Quantity { get; set; }
    }
}