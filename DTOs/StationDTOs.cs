namespace Bus_ticketing_Backend.DTOs
{
    public class StationDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; }
        public string StationName { get; set; }
        public string StreetEn { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class CreateStationDto
    {
        public string CityId { get; set; }
        public string NameEn { get; set; }
        public string StationName { get; set; }
        public string StreetEn { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class UpdateStationDto
    {
        public string NameEn { get; set; }
        public string StationName { get; set; }
        public string StreetEn { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
