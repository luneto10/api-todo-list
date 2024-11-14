using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TodoListAPI.Models;

namespace TodoListAPI.Services;

public class MongoDBServices
{
    private readonly IMongoCollection<TaskModel> _todosCollection;

    public MongoDBServices(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _todosCollection = database.GetCollection<TaskModel>(mongoDBSettings.Value.CollectionName);
    }

    public async Task CreateAsync(TaskModel task)
    {
        await _todosCollection.InsertOneAsync(task);
    }

    public async Task<TaskModel> GetById(string id)
    {
        FilterDefinition<TaskModel> filter = Builders<TaskModel>.Filter.Eq("Id", id);
    
        var taskModel = await _todosCollection.Find(filter).FirstOrDefaultAsync();
        return taskModel;
    }
    
    public async Task<List<TaskModel>> GetAsync()
    {
        return await _todosCollection.Find(new BsonDocument()).ToListAsync();
    }
    
    public async Task UpdateAsync(string id, TaskModel task)
    {
        FilterDefinition<TaskModel> filter = Builders<TaskModel>.Filter.Eq("Id", id);

        UpdateDefinition<TaskModel> update = Builders<TaskModel>.Update
            .Set(t => t.Text, task.Text)
            .Set(t => t.Completed, task.Completed);

        await _todosCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<TaskModel> filter = Builders<TaskModel>.Filter.Eq("Id", id);

        await _todosCollection.DeleteOneAsync(filter);
    }

    public async Task DeleteAllAsync()
    {
        await _todosCollection.DeleteManyAsync(FilterDefinition<TaskModel>.Empty);
    }

    public async Task DeleteAllFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be null or empty", nameof(url));
        }
        
        string decodedUrl = Uri.UnescapeDataString(url);

        FilterDefinition<TaskModel> filter = Builders<TaskModel>.Filter.Eq("Url", decodedUrl);
        
        await _todosCollection.DeleteManyAsync(filter);
    }
    
    public async Task<List<TaskModel>> GetByUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be null or empty", nameof(url));
        }

        Console.WriteLine($"Searching for tasks with URL: {url}");

        FilterDefinition<TaskModel> filter = Builders<TaskModel>.Filter.Regex(t => t.Url, url);
        List<TaskModel> tasks = await _todosCollection.Find(filter).ToListAsync();

        Console.WriteLine($"Tasks found: {tasks.Count}");
    
        return tasks;
    }
        
}