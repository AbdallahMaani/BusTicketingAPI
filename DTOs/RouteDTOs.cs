using System.ComponentModel.DataAnnotations;

namespace Bus_ticketing_Backend.DTOs
{
    public class RouteDto
    {
        public Guid RouteId { get; set; }
        public string OriginId { get; set; }
        public string DestinationId { get; set; }
        public int DistanceKm { get; set; }
        public double DurationHrs { get; set; }
    }

    public class CreateRouteDto
    {
        
        [Required]
        public string OriginId { get; set; }

        [Required]
        public string DestinationId { get; set; }

        public int DistanceKm { get; set; }
        public double DurationHrs { get; set; }
    }

    public class UpdateRouteDto
    {
        public string OriginId { get; set; }
        public string DestinationId { get; set; }
        public int DistanceKm { get; set; }
        public double DurationHrs { get; set; }
    }
}