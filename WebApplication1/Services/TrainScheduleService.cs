//Train Schedule Service
// This service manages train schedules and related operations.

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

    // Get all train schedules
    public async Task<List<TrainSchedule>> GetAllAsync()
        {
            return await _scheduleCollection.Find(new BsonDocument()).ToListAsync();
        }

    // Get a train schedule by ID
    public async Task<TrainSchedule> GetByIdAsync(string id)
        {
            var filter = Builders<TrainSchedule>.Filter.Eq(schedule => schedule.Id, id);
            return await _scheduleCollection.Find(filter).FirstOrDefaultAsync();
        }

    // Create a new train schedule
    public async Task<string> CreateAsync(TrainSchedule schedule)
        {
            await _scheduleCollection.InsertOneAsync(schedule);
            return schedule.Id;
        }

    // Update an existing train schedule
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
            existingSchedule.From = schedule.From;

            var filter = Builders<TrainSchedule>.Filter.Eq(s => s.Id, id);
            await _scheduleCollection.ReplaceOneAsync(filter, existingSchedule);
        }

    // Delete a train schedule by ID
    public async Task DeleteAsync(string id)
        {
            var filter = Builders<TrainSchedule>.Filter.Eq(schedule => schedule.Id, id);
            await _scheduleCollection.DeleteOneAsync(filter);
        }
    
}