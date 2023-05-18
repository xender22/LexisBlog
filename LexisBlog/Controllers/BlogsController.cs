using LexisBlog.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace LexisBlog.Controllers;

[Route("api/blogs")]
[ApiController]
public class BlogsController : ControllerBase
{
    private readonly BlogRepository _blogRepository;
    private readonly UserRepository _userRepository;

    public BlogsController(BlogRepository blogRepository, UserRepository userRepository)
    {
        _userRepository = userRepository;
        _blogRepository = blogRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog(Blog blog)
    {
        if (!ObjectId.TryParse(blog.AuthorId, out var objectId)) return BadRequest("Invalid or non existent Author");
        // Check if the user exists
        var user = await _userRepository.GetUser(objectId);
        if(user == null)
            return BadRequest("Invalid or non existent Author");
        
        var createdBlog = await _blogRepository.CreateBlog(blog);
        return Ok(createdBlog);
    }

    [HttpGet("{blogId}")]
    public async Task<IActionResult> GetBlog(string blogId)
    {
        if (!ObjectId.TryParse(blogId, out var objectId))
            return BadRequest("Invalid blog ID");

        var blog = await _blogRepository.GetBlog(objectId);
        if (blog.Id == ObjectId.Empty)
            return NotFound();

        return Ok(blog);
    }
    
    [HttpGet("{blogId}/withAuthor")]
    public async Task<IActionResult> GetBlogWithAuthor(string blogId)
    {
        if (!ObjectId.TryParse(blogId, out var objectId))
            return BadRequest("Invalid blog ID");

        var blog = await _blogRepository.GetBlogWithAuthor(objectId);
        if (blog.Id == ObjectId.Empty)
            return NotFound();

        return Ok(blog);
    }
    
    [HttpGet("count/{authorId}")]
    public async Task<IActionResult> CountBlogRecordsByAuthor(string authorId)
    {
        if (!ObjectId.TryParse(authorId, out var objectId))
            return BadRequest("Invalid author ID");

        var count = await _blogRepository.CountBlogRecordsByAuthor(authorId);

        return Ok(count);
    }

    
    [HttpPut("{blogId}")]
    public async Task<IActionResult> UpdateBlog(string blogId, [FromBody] string updatedText)
    {
        if (!ObjectId.TryParse(blogId, out var objectId))
            return BadRequest("Invalid blog ID");

        var blog = await _blogRepository.GetBlog(objectId);
        if (blog.Id == ObjectId.Empty)
            return NotFound();

        blog.Text = updatedText;

        var success = await _blogRepository.UpdateBlog(blog);
        if (!success)
            return StatusCode(500, "Failed to update blog");

        return NoContent();
    }

    [HttpDelete("{blogId}")]
    public async Task<IActionResult> DeleteBlog(string blogId)
    {
        if (!ObjectId.TryParse(blogId, out var objectId))
            return BadRequest("Invalid blog ID");

        var blog = await _blogRepository.GetBlog(objectId);
        if (blog.Id == ObjectId.Empty)
            return NotFound();

        var success = await _blogRepository.DeleteBlog(blog.Id);
        if (!success)
            return StatusCode(500, "Failed to delete blog");

        return NoContent();
    }
}