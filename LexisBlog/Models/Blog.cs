using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace LexisBlog.Models;

public class Blog
{
    public ObjectId Id { get; set; }
    public string? AuthorId { get; set; }
    public DateTime PublishedOn { get; set; }
    public string? Text { get; set; }
    public DateTime CreatedOn { get; set; }
    public User? Author { get; set; }
}