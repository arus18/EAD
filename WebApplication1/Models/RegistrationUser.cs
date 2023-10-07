namespace WebApplication1.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class RegistrationUser
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("UserName")]
    public string UserName { get; set; }

    [BsonElement("Password")]
    public string Password { get; set; }

    [BsonElement("NIC")]
    public string NIC { get; set; }

    [BsonElement("IsActivated")]
    public bool IsActivated { get; set; }
}

