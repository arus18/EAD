using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace WebApplication1.Models;

public class Station
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string StationName { get; set; }

    public string ArrivalTime { get; set; }

    public string DepartureTime { get; set; }
}