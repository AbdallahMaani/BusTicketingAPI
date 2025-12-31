using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_ticketingAPI.Entities
{
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Guid TripId { get; set; }
        [ForeignKey("TripId")]
        public Trip Trip { get; set; }

        public DateTime BookingDate { get; set; }
        public string Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePaid { get; set; }
    }
}