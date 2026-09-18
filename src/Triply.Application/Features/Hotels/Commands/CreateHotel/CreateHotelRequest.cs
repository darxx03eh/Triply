namespace Triply.Application.Features.Hotels.Commands.CreateHotel;

public class CreateHotelRequest
{
    public string Name { get; set; }
    public Guid CityId { get; set; }
    public byte StarRating  { get; set; }
    public string Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}