using Microsoft.Extensions.Options; // Import the Microsoft.Extensions.Options namespace for working with options/configuration.
using MongoDB.Bson; // Import the MongoDB.Bson namespace for working with BSON objects.
using MongoDB.Driver; // Import the MongoDB.Driver namespace for MongoDB database operations.
using WebApplication1.Models; // Import the models required for this service.

namespace WebApplication1.Services
{
    public class TrainDetailService
    {
        private readonly IMongoCollection<TrainDetail> _detailCollection; // Collection for managing train details.

        public TrainDetailService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            // Constructor for the TrainDetailService, initializes MongoDB database connection.
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _detailCollection = database.GetCollection<TrainDetail>(mongoDBSettings.Value.TrainDetailCollectionName);
        }

        // Get all train details
        public async Task<List<TrainDetail>> GetAllAsync()
        {
            // Retrieve all train details from the collection asynchronously.
            return await _detailCollection.Find(new BsonDocument()).ToListAsync();
        }

        // Get a train detail by ID
        public async Task<TrainDetail> GetByIdAsync(string id)
        {
            // Retrieve a train detail by its ID asynchronously.
            var filter = Builders<TrainDetail>.Filter.Eq(detail => detail.Id, id);
            return await _detailCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Create a new train detail
        public async Task<string> CreateAsync(TrainDetail detail)
        {
            // Create a new train detail and insert it into the collection asynchronously.
            await _detailCollection.InsertOneAsync(detail);
            return detail.Id;
        }

        // Update an existing train detail
        public async Task UpdateAsync(string id, TrainDetail detail)
        {
            // Update an existing train detail by its ID asynchronously.
            var existingDetail = await GetByIdAsync(id);
            if (existingDetail == null)
            {
                throw new ArgumentException($"Train detail with ID {id} not found.");
            }

            // Update the fields of the existing train detail as needed.
            existingDetail.TrainNo = detail.TrainNo;
            existingDetail.TrainName = detail.TrainName;
            existingDetail.FirstClassCapacity = detail.FirstClassCapacity;
            existingDetail.SecondClassCapacity = detail.SecondClassCapacity;
            existingDetail.ThirdClassCapacity = detail.ThirdClassCapacity;
            existingDetail.Type = detail.Type;
            existingDetail.IsPublished = detail.IsPublished;
            existingDetail.IsActive = detail.IsActive;

            var filter = Builders<TrainDetail>.Filter.Eq(d => d.Id, id);
            await _detailCollection.ReplaceOneAsync(filter, existingDetail);
        }

        // Delete a train detail by ID
        public async Task DeleteAsync(string id)
        {
            // Delete a train detail by its ID asynchronously.
            var filter = Builders<TrainDetail>.Filter.Eq(detail => detail.Id, id);
            await _detailCollection.DeleteOneAsync(filter);
        }
    }
}
