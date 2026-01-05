using System.ComponentModel.DataAnnotations;

namespace Bus_ticketing_Backend.DTOs
{
    public class CityDto
    {
        public string Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public List<StationDto> busStations { get; set; } = new List<StationDto>();

    }
}