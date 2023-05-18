using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace LexisBlog.Models;

public class User
{
    public ObjectId Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}