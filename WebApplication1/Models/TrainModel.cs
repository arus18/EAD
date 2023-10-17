namespace WebApplication1.Models; // Define the namespace for the model class.

using MongoDB.Bson; // Import the MongoDB.Bson namespace for working with BSON objects.
using MongoDB.Bson.Serialization.Attributes; // Import the necessary attributes for BSON serialization.

public class TrainDetail
{
    [BsonId] // This attribute indicates that the property serves as the BSON document's ID field.
    [BsonRepresentation(BsonType.ObjectId)] // This specifies the representation of the ID as an ObjectId.
    public string? Id { get; set; } // Property to store the unique identifier for the train detail.

    public string TrainNo { get; set; } // Property to store the train number.

    public string TrainName { get; set; } // Property to store the name of the train.

    public int FirstClassCapacity { get; set; } // Property to store the first-class capacity.

    public int SecondClassCapacity { get; set; } // Property to store the second-class capacity.

    public int ThirdClassCapacity { get; set; } // Property to store the third-class capacity.

    public string Type { get; set; } // Property to store the type of the train.

    public bool IsPublished { get; set; } // Property to indicate if the train detail is published.

    public bool IsActive { get; set; } // Property to indicate if the train is active.
}
