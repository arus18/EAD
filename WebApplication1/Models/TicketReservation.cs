//Ticket Reservation 

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models;
public class TicketReservation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string ReferenceId { get; set; }
    
    public DateTime ReservationDate { get; set; }
    
    public string? UserId { get; set; }
    
    public string FromDestination { get; set; } // Add FromDestination property
    
    public string ToDestination { get; set; } // Add ToDestination property
}

