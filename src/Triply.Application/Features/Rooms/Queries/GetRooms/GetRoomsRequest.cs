using Sieve.Models;

namespace Triply.Application.Features.Rooms.Queries.GetRooms;

/// <summary>Request payload used to get rooms.</summary>
public class GetRoomsRequest : SieveModel
{
    /// <summary>Requested check-in date used to calculate availability.</summary>
    public DateOnly? CheckIn { get; set; }
    /// <summary>Requested check-out date used to calculate availability.</summary>
    public DateOnly? CheckOut { get; set; }
}
