using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class StationService
    {
        private readonly IMongoCollection<Station> _stationCollection;

        public StationService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _stationCollection = database.GetCollection<Station>(mongoDBSettings.Value.StationCollectionName);
        }

        public async Task<List<Station>> GetAllAsync()
        {
            return await _stationCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Station> GetByIdAsync(string id)
        {
            var filter = Builders<Station>.Filter.Eq(station => station.Id, id);
            return await _stationCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<string> CreateAsync(Station station)
        {
            await _stationCollection.InsertOneAsync(station);
            return station.Id;
        }

        public async Task UpdateAsync(string id, Station station)
        {
            var existingStation = await GetByIdAsync(id);
            if (existingStation == null)
            {
                throw new ArgumentException($"Station with ID {id} not found.");
            }

            // Update fields as needed.
            existingStation.StationName = station.StationName;
            existingStation.ArrivalTime = station.ArrivalTime;
            existingStation.DepartureTime = station.DepartureTime;

            var filter = Builders<Station>.Filter.Eq(s => s.Id, id);
            await _stationCollection.ReplaceOneAsync(filter, existingStation);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Station>.Filter.Eq(station => station.Id, id);
            await _stationCollection.DeleteOneAsync(filter);
        }
    }
}
