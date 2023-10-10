//Train Schedule

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace WebApplication1.Models;

public class TrainSchedule
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string TrainName { get; set; }

    public DateTime DepartureTime { get; set; }

    public string Destination { get; set; }
}