//User Service
// This service manages user-related operations, including user registration and activation.

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using WebApplication1.Models;

public class UserService
{
    private readonly IMongoCollection<RegistrationUser> _userCollection;

    public UserService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _userCollection = database.GetCollection<RegistrationUser>(mongoDBSettings.Value.UserCollectionName);
        var keys = Builders<RegistrationUser>.IndexKeys.Ascending(u => u.NIC);
        var uniqueIndex = new CreateIndexModel<RegistrationUser>(keys, new CreateIndexOptions { Unique = true });
        _userCollection.Indexes.CreateOne(uniqueIndex);
    }

    // Get all registration users
    public async Task<List<RegistrationUser>> GetAllAsync()
    {
        return await _userCollection.Find(new BsonDocument()).ToListAsync();
    }

    // Get a registration user by ID
    public async Task<RegistrationUser> GetByIdAsync(string id)
    {
        return await _userCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
    }

    // Create a new registration user
    public async Task<RegistrationUser> CreateAsync(RegistrationUser user)
    {
        await _userCollection.InsertOneAsync(user);
        return user;
    }

    // Update an existing registration user
    public async Task UpdateAsync(string id, RegistrationUser user)
    {
        await _userCollection.ReplaceOneAsync(u => u.Id == id, user);
    }

    // Delete a registration user by ID
    public async Task DeleteAsync(string id)
    {
        await _userCollection.DeleteOneAsync(user => user.Id == id);
    }

    // Activate or Deactivate a user by ID
    public async Task SetActivationAsync(string id, bool isActivated)
    {
        var filter = Builders<RegistrationUser>.Filter.Eq(u => u.Id, id);
        var update = Builders<RegistrationUser>.Update.Set(u => u.IsActivated, isActivated);
        await _userCollection.UpdateOneAsync(filter, update);
    }

    // Get a user by NIC
    public async Task<RegistrationUser> GetUserByNicAsync(string nic)
    {
        var filter = Builders<RegistrationUser>.Filter.Eq(user => user.NIC, nic);
        return await _userCollection.Find(filter).FirstOrDefaultAsync();
    }

}