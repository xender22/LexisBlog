using LexisBlog.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LexisBlog.Models;

public class UserRepository
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserRepository(IConfiguration config)
        {
            var settings = config.GetSection("MongoDbSettings").Get<MongoSettings>();

            var clientSettings = MongoClientSettings.FromConnectionString(settings?.ConnectionString);
            var mongoClient = new MongoClient(clientSettings);
            var database = mongoClient.GetDatabase(settings?.DatabaseName);

            _userCollection = database.GetCollection<User>("Users");
        }

        public async Task<User> CreateUser(User user)
        {
            await _userCollection.InsertOneAsync(user);
            return user;
        }

        public async Task<User> GetUser(ObjectId userId)
        {
            var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            return user;
        }

        public async Task<bool> UpdateUser(User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, updatedUser.Id);
            var update = Builders<User>.Update.Combine(GetUpdateDefinitions(updatedUser));

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);
            return updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUser(ObjectId userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var deleteResult = await _userCollection.DeleteOneAsync(filter);
            return deleteResult.DeletedCount > 0;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _userCollection.Find(_ => true).ToListAsync();
            return users;
        }

        private IEnumerable<UpdateDefinition<User>> GetUpdateDefinitions(User updatedUser)
        {
            var updateDefinitions = new List<UpdateDefinition<User>>();

            if (!string.IsNullOrEmpty(updatedUser.FirstName))
                updateDefinitions.Add(Builders<User>.Update.Set(x => x.FirstName, updatedUser.FirstName));

            if (!string.IsNullOrEmpty(updatedUser.LastName))
                updateDefinitions.Add(Builders<User>.Update.Set(x => x.LastName, updatedUser.LastName));

            // Add more conditions for other fields you want to update

            return updateDefinitions;
        }
    }