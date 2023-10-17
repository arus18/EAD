using Microsoft.Extensions.Options; // Import the Microsoft.Extensions.Options namespace for working with options/configuration.
using MongoDB.Bson; // Import the MongoDB.Bson namespace for working with BSON objects.
using MongoDB.Driver; // Import the MongoDB.Driver namespace for MongoDB database operations.
using System; // Import the System namespace for fundamental types and exceptions.
using System.Collections.Generic; // Import the System.Collections.Generic namespace for lists.
using System.Threading.Tasks; // Import the System.Threading.Tasks namespace for asynchronous operations.
using WebApplication1.Models; // Import the models required for this service.

namespace WebApplication1.Services
{
    public class StationService
    {
        private readonly IMongoCollection<Station> _stationCollection; // Collection for managing stations.

        public StationService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            // Constructor for the StationService, initializes MongoDB database connection.
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _stationCollection = database.GetCollection<Station>(mongoDBSettings.Value.StationCollectionName);
        }

        public async Task<List<Station>> GetAllAsync()
        {
            // Retrieve all stations from the collection asynchronously.
            return await _stationCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Station> GetByIdAsync(string id)
        {
            // Retrieve a station by its ID asynchronously.
            var filter = Builders<Station>.Filter.Eq(station => station.Id, id);
            return await _stationCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<string> CreateAsync(Station station)
        {
            // Create a new station and insert it into the collection asynchronously.
            await _stationCollection.InsertOneAsync(station);
            return station.Id;
        }

        public async Task UpdateAsync(string id, Station station)
        {
            // Update an existing station by its ID asynchronously.
            var existingStation = await GetByIdAsync(id);
            if (existingStation == null)
            {
                throw new ArgumentException($"Station with ID {id} not found.");
            }

            // Update the fields of the existing station.
            existingStation.StationName = station.StationName;
            existingStation.ArrivalTime = station.ArrivalTime;
            existingStation.DepartureTime = station.DepartureTime;

            var filter = Builders<Station>.Filter.Eq(s => s.Id, id);
            await _stationCollection.ReplaceOneAsync(filter, existingStation);
        }

        public async Task DeleteAsync(string id)
        {
            // Delete a station by its ID asynchronously.
            var filter = Builders<Station>.Filter.Eq(station => station.Id, id);
            await _stationCollection.DeleteOneAsync(filter);
        }
    }
}
