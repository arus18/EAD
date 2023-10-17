using MongoDB.Bson; // Import the MongoDB.Bson namespace for working with BSON objects.
using MongoDB.Bson.Serialization.Attributes; // Import the necessary attributes for BSON serialization.
namespace WebApplication1.Models; // Define the namespace for the model class.

public class Station
{
    [BsonId] // This attribute indicates that the property serves as the BSON document's ID field.
    [BsonRepresentation(BsonType.ObjectId)] // This specifies the representation of the ID as an ObjectId.
    public string? Id { get; set; } // Property to store the unique identifier for the station.

    public string StationName { get; set; } // Property to store the name of the station.

    public string ArrivalTime { get; set; } // Property to store the arrival time at the station.

    public string DepartureTime { get; set; } // Property to store the departure time from the station.
}
