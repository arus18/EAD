// Train Schedule
// This class represents a model for train schedule information.

using MongoDB.Bson; // Import the MongoDB.Bson namespace for working with BSON objects.
using MongoDB.Bson.Serialization.Attributes; // Import the necessary attributes for BSON serialization.
namespace WebApplication1.Models; // Define the namespace for the model class.

public class TrainSchedule
{
    [BsonId] // This attribute indicates that the property serves as the BSON document's ID field.
    [BsonRepresentation(BsonType.ObjectId)] // This specifies the representation of the ID as an ObjectId.
    public string? Id { get; set; } // Property to store the unique identifier for the train schedule.

    public string TrainName { get; set; } // Property to store the name of the train.

    public DateTime DepartureTime { get; set; } // Property to store the departure time of the train.

    public string From { get; set; } // Property to store the origin or starting point of the train route.

    public string Destination { get; set; } // Property to store the destination or ending point of the train route.
}
