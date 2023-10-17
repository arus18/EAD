namespace WebApplication1.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class TrainDetail
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string TrainNo { get; set; }
    
    public string TrainName { get; set; }

    public int FirstClassCapacity { get; set; }

    public int SecondClassCapacity { get; set; }

    public int ThirdClassCapacity { get; set; }

    public string Type { get; set; }

    public bool IsPublished { get; set; }

    public bool IsActive { get; set; }
}
