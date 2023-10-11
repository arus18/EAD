//Ticket Booking Service
// This service manages ticket reservations and related operations.

using WebApplication1.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

public class TicketBookingService
{
    private readonly IMongoCollection<TicketReservation> _reservationCollection;

    public TicketBookingService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _reservationCollection = database.GetCollection<TicketReservation>(mongoDBSettings.Value.ReservationCollectionName);
    }

    // Get all ticket reservations
    public async Task<List<TicketReservation>> GetAsync()
    {
        return await _reservationCollection.Find(new BsonDocument()).ToListAsync();
    }

    // Get a ticket reservation by ID
    public async Task<TicketReservation> GetByIdAsync(string id)
    {
        var filter = Builders<TicketReservation>.Filter.Eq(reservation => reservation.Id, id);
        return await _reservationCollection.Find(filter).FirstOrDefaultAsync();
    }

    // Create a new ticket reservation
    public async Task<string> CreateAsync(TicketReservation reservation)
    {
        // Validation: Check if the reservation date is within 30 days from the booking date.
        DateTime bookingDate = DateTime.UtcNow;
        if (reservation.ReservationDate <= bookingDate || reservation.ReservationDate > bookingDate.AddDays(30))
        {
            throw new ArgumentException("Reservation date should be within 30 days from the booking date.");
        }

        // Validation: Check if the maximum number of reservations (4) per reference ID is not exceeded.
        //get number of reservations by userid from,to,time
        int maxReservationsPerReferenceId = 4;
        long existingReservations = await GetReservationsCountByIdFromToTime(reservation.ReferenceId,reservation.FromDestination,reservation.ToDestination,reservation.ReservationDate);
        if (existingReservations >= maxReservationsPerReferenceId)
        {
            throw new ArgumentException($"Maximum {maxReservationsPerReferenceId} reservations per reference ID is allowed.");
        }

        await _reservationCollection.InsertOneAsync(reservation);
        return reservation.Id;
    }

    // Update an existing ticket reservation
    public async Task UpdateAsync(string id, TicketReservation reservation)
    {
        var existingReservation = await GetByIdAsync(id);
        if (existingReservation == null)
        {
            throw new ArgumentException($"Reservation with ID {id} not found.");
        }

        // Validation: Check if the reservation can be updated (at least 5 days before the reservation date).
        DateTime minimumUpdateDate = existingReservation.ReservationDate.AddDays(-5);
        if (DateTime.UtcNow >= minimumUpdateDate)
        {
            throw new ArgumentException("Reservation can only be updated at least 5 days before the reservation date.");
        }

        // Perform the update.
        // ...
    }

    // Delete a ticket reservation by ID
    public async Task DeleteAsync(string id)
    {
        var filter = Builders<TicketReservation>.Filter.Eq(reservation => reservation.Id, id);
        await _reservationCollection.DeleteOneAsync(filter);
    }

    // Get the count of reservations by ID, from destination, to destination, and date
    public async Task<long> GetReservationsCountByIdFromToTime(string id, string fromDestination, string toDestination, DateTime date)
    {
        var filter = Builders<TicketReservation>.Filter.And(
            Builders<TicketReservation>.Filter.Eq(reservation => reservation.Id, id),
            Builders<TicketReservation>.Filter.Eq(reservation => reservation.FromDestination, fromDestination),
            Builders<TicketReservation>.Filter.Eq(reservation => reservation.ToDestination, toDestination),
            Builders<TicketReservation>.Filter.Eq(reservation => reservation.ReservationDate, date)
        );

        return await _reservationCollection.Find(filter).CountDocumentsAsync();
    }


    // Get reservations by user ID
    public async Task<List<TicketReservation>> GetReservationsByUserIdAsync(string userId)
    {
        var filter = Builders<TicketReservation>.Filter.Eq(reservation => reservation.UserId, userId);
        return await _reservationCollection.Find(filter).ToListAsync();
    }


}

