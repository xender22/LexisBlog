using LexisBlog.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LexisBlog.Models;

public class BlogRepository
{
    private readonly IMongoCollection<Blog> _blogCollection;
    private readonly IMongoCollection<User> _userCollection;

    public BlogRepository(IConfiguration config)
    {
        var settings = config.GetSection("MongoDbSettings").Get<MongoSettings>();

        var clientSettings = MongoClientSettings.FromConnectionString(settings?.ConnectionString);
        var mongoClient = new MongoClient(clientSettings);
        var database = mongoClient.GetDatabase(settings?.DatabaseName);

        _blogCollection = database.GetCollection<Blog>("Blogs");
        _userCollection = database.GetCollection<User>("Users");
    }

    public async Task<Blog> CreateBlog(Blog blog)
    {
        await _blogCollection.InsertOneAsync(blog);
        return blog;
    }

    public async Task<Blog> GetBlog(ObjectId blogId)
    {
        return await _blogCollection.Find(b => b.Id == blogId).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateBlog(Blog updatedBlog)
    {
        var filter = Builders<Blog>.Filter.Eq(x => x.Id, updatedBlog.Id);
        var update = Builders<Blog>.Update.Set(x => x.Text, updatedBlog.Text);

        var updateResult = await _blogCollection.UpdateOneAsync(filter, update);
        return updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteBlog(ObjectId blogId)
    {
        var filter = Builders<Blog>.Filter.Eq(x => x.Id, blogId);
        var deleteResult = await _blogCollection.DeleteOneAsync(filter);
        return deleteResult.DeletedCount > 0;
    }

    public async Task<long> CountBlogRecordsByAuthor(string id)
    {
        var filter = Builders<Blog>.Filter.Eq(x => x.AuthorId, id);
        var count = await _blogCollection.CountDocumentsAsync(filter);
        return count;
    }
    
    public async Task<Blog> GetBlogWithAuthor(ObjectId blogId)
    {
        var filter = Builders<Blog>.Filter.Eq(x => x.Id, blogId);
        var blog = await _blogCollection.Find(filter).FirstOrDefaultAsync();

       
        var author = await _userCollection.Find(u => u.Id == new ObjectId(blog.AuthorId)).FirstOrDefaultAsync();
        blog.Author = author;
        

        return blog;
    }

}