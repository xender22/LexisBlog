using LexisBlog.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace LexisBlog.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserRepository _repository;

    public UsersController(UserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        var createdUser = await _repository.CreateUser(user);
        return Ok(createdUser);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        if (!ObjectId.TryParse(userId, out var objectId))
            return BadRequest("Invalid user ID");

        var user = await _repository.GetUser(objectId);
        if (user.Id == ObjectId.Empty)
            return NotFound();

        return Ok(user);
    }
    
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, User updatedUser)
    {
        if (!ObjectId.TryParse(userId, out var objectId))
            return BadRequest("Invalid user ID");

        updatedUser.Id = objectId;

        bool isUpdated = await _repository.UpdateUser(updatedUser);
        if (isUpdated)
            return Ok(updatedUser);
        else
            return NotFound();
    }
    
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        if (!ObjectId.TryParse(userId, out var objectId))
            return BadRequest("Invalid user ID");

        bool isDeleted = await _repository.DeleteUser(objectId);
        if (isDeleted)
            return NoContent();
        else
            return NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _repository.GetAllUsers();
        return Ok(users);
    }
}