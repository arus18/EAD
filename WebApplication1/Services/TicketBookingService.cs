using WebApplication1.Models;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Models; // Assuming your TicketReservation model is in this namespace

public class TicketBookingService
{
    private readonly IMongoCollection<TicketReservation> _reservationCollection;

    public TicketBookingService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _reservationCollection = database.GetCollection<TicketReservation>(mongoDBSettings.Value.ReservationCollectionName);
    }

    public async Task<List<TicketReservation>> GetAsync()
    {
        return await _reservationCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<TicketReservation> GetByIdAsync(string id)
    {
        var filter = Builders<TicketReservation>.Filter.Eq(reservation => reservation.Id, id);
        return await _reservationCollection.Find(filter).FirstOrDefaultAsync();
    }

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
        long existingReservations = await GetReservationsCountByReferenceId(reservation.ReferenceId);
        if (existingReservations >= maxReservationsPerReferenceId)
        {
            throw new ArgumentException($"Maximum {maxReservationsPerReferenceId} reservations per reference ID is allowed.");
        }

        await _reservationCollection.InsertOneAsync(reservation);
        return reservation.Id;
    }


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


    public async Task DeleteAsync(string id)
    {
        var filter = Builders<TicketReservation>.Filter.Eq(reservation => reservation.Id, id);
        await _reservationCollection.DeleteOneAsync(filter);
    }
    
    public async Task<long> GetReservationsCountByReferenceId(string referenceId)
    {
        var filter = Builders<TicketReservation>.Filter.Eq(reservation => reservation.ReferenceId, referenceId);
        return await _reservationCollection.Find(filter).CountDocumentsAsync();
    }
    
    public async Task<List<TicketReservation>> GetReservationsByUserIdAsync(string userId)
    {
        var filter = Builders<TicketReservation>.Filter.Eq(reservation => reservation.UserId, userId);
        return await _reservationCollection.Find(filter).ToListAsync();
    }


}

