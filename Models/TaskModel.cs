using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoListAPI.Models;

public class TaskModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string Text { get; set; }
    
    public bool Completed { get; set; }

    public string Url { get; set; }
    
}