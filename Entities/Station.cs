using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_ticketingAPI.Entities
{
    public class Station
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }

        public string NameEn { get; set; }
        public string StationName { get; set; }
        public string StreetEn { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}