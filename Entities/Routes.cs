using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_ticketingAPI.Entities
{
    public class Routes
    {
        [Key]
        public Guid RouteId { get; set; } = Guid.NewGuid();

        [Required]
        public string OriginId { get; set; }

        [ForeignKey("OriginId")]
        public City Origin { get; set; }

        [Required]
        public string DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public City Destination { get; set; }

        public int DistanceKm { get; set; }
        public double DurationHrs { get; set; }
    }
}