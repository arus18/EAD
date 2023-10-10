using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class TrainScheduleService
    {
        private readonly IMongoCollection<TrainSchedule> _scheduleCollection;

        public TrainScheduleService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _scheduleCollection = database.GetCollection<TrainSchedule>(mongoDBSettings.Value.ScheduleCollectionName);
        }

        public async Task<List<TrainSchedule>> GetAllAsync()
        {
            return await _scheduleCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<TrainSchedule> GetByIdAsync(string id)
        {
            var filter = Builders<TrainSchedule>.Filter.Eq(schedule => schedule.Id, id);
            return await _scheduleCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<string> CreateAsync(TrainSchedule schedule)
        {
            await _scheduleCollection.InsertOneAsync(schedule);
            return schedule.Id;
        }

        public async Task UpdateAsync(string id, TrainSchedule schedule)
        {
            var existingSchedule = await GetByIdAsync(id);
            if (existingSchedule == null)
            {
                throw new ArgumentException($"Train schedule with ID {id} not found.");
            }

            // Update fields as needed.
            existingSchedule.TrainName = schedule.TrainName;
            existingSchedule.DepartureTime = schedule.DepartureTime;
            existingSchedule.Destination = schedule.Destination;

            var filter = Builders<TrainSchedule>.Filter.Eq(s => s.Id, id);
            await _scheduleCollection.ReplaceOneAsync(filter, existingSchedule);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<TrainSchedule>.Filter.Eq(schedule => schedule.Id, id);
            await _scheduleCollection.DeleteOneAsync(filter);
        }
    
}