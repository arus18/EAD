using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class TrainDetailService
    {
        private readonly IMongoCollection<TrainDetail> _detailCollection;

        public TrainDetailService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _detailCollection = database.GetCollection<TrainDetail>(mongoDBSettings.Value.TrainDetailCollectionName);
        }

        // Get all train details
        public async Task<List<TrainDetail>> GetAllAsync()
        {
            return await _detailCollection.Find(new BsonDocument()).ToListAsync();
        }

        // Get a train detail by ID
        public async Task<TrainDetail> GetByIdAsync(string id)
        {
            var filter = Builders<TrainDetail>.Filter.Eq(detail => detail.Id, id);
            return await _detailCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Create a new train detail
        public async Task<string> CreateAsync(TrainDetail detail)
        {
            await _detailCollection.InsertOneAsync(detail);
            return detail.Id;
        }

        // Update an existing train detail
        public async Task UpdateAsync(string id, TrainDetail detail)
        {
            var existingDetail = await GetByIdAsync(id);
            if (existingDetail == null)
            {
                throw new ArgumentException($"Train detail with ID {id} not found.");
            }

            // Update fields as needed.
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
            var filter = Builders<TrainDetail>.Filter.Eq(detail => detail.Id, id);
            await _detailCollection.DeleteOneAsync(filter);
        }
    }
}
