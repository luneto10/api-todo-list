using System.Xml;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TodoListAPI.Models;
using TodoListAPI.Services;

namespace TodoListAPI.Controllers;

[Controller]
[Route("api/[controller]")]
public class TaskController : Controller
{
    private readonly MongoDBServices _mongoDbServices;

    public TaskController(MongoDBServices mongoDbServices)
    {
        _mongoDbServices = mongoDbServices;
    }

    [HttpGet]
    public async Task<List<TaskModel>> Get()
    {
        return await _mongoDbServices.GetAsync();
    }
    
    [HttpGet("{url}")]
    public async Task<IActionResult> GetTasksByUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest("URL cannot be null or empty.");
        }

        // Decode URL if it was encoded in the frontend
        string decodedUrl = Uri.UnescapeDataString(url);

        List<TaskModel> tasks = await _mongoDbServices.GetByUrl(decodedUrl);

        return Ok(tasks);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TaskModel task)
    {
        
        await _mongoDbServices.CreateAsync(task);

        if (task.Id == null)
        {
            return BadRequest("Task Id cannot be null after creation.");
        }

        return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TaskModel task)
    {
        await _mongoDbServices.UpdateAsync(id, task);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _mongoDbServices.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        await _mongoDbServices.DeleteAllAsync();
        return NoContent();
    }
    

    [HttpDelete("by-url/{url}")]
    public async Task<IActionResult> DeleteByUrl(string url)
    {
        await _mongoDbServices.DeleteAllFromUrl(url);
        return NoContent();
    }
}