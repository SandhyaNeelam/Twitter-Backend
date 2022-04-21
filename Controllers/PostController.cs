using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twitter_BE.DTOs;
using Twitter_BE.Models;
using Twitter_BE.Repositories;
using Twitter_BE.Utilities;

namespace Twitter_BE.Controllers;

[ApiController]
[Route("api/post")]
[Authorize]

public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostRepository _post;

    public PostController(ILogger<PostController> logger, IPostRepository post)
    {
        _logger = logger;
        _post = post;
    }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == UserConstants.Id).First().Value);
    }



    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost([FromBody] PostCreateDTO Data)
    {
        var User_Id = GetUserIdFromClaims(User.Claims);

        var postCount = (await _post.GetByUserId(User_Id)).Count;
        if (postCount >= 5)
            return BadRequest("Post limit exceded");

        var toCreatePost = new Post
        {
            Title = Data.Title.Trim(),
            UserId = User_Id,
        };
        var createdPost = await _post.Create(toCreatePost);
        return StatusCode(201, createdPost);
    }

    [HttpPut("{post_id}")]
    public async Task<ActionResult> UpdatePost([FromRoute] int post_id,
    [FromBody] PostUpdateDTO Data)
    {
        var User_Id = GetUserIdFromClaims(User.Claims);

        var existingPost = await _post.GetById(post_id);
        if (existingPost is null)
            return NotFound();

        if (existingPost.UserId != User_Id)
            return StatusCode(403, "You cannot update others Posts");

        var toUpdatePost = existingPost with
        {
            Title = Data.Title is null ? existingPost.Title : Data.Title.Trim(),
            // UpdatedAt = Data.UpdatedAt
        };
        await _post.Update(toUpdatePost);
        return NoContent();
    }

    [HttpDelete("{post_id}")]
    public async Task<ActionResult> DeletePost(int post_id)
    {
        var User_Id = GetUserIdFromClaims(User.Claims);

        var existingPost = await _post.GetById(post_id);
        if (existingPost is null)
            return NotFound();

        if (existingPost.UserId != User_Id)
            return StatusCode(403, "You cannot delete others Posts");

        await _post.Delete(post_id);
        return NoContent();
    }

    [HttpGet()]
    public async Task<ActionResult<List<Post>>> GetAllPosts()
    {
        var allPosts = await _post.GetAll();
        return Ok(allPosts);
    }

    [HttpGet("{PostId}")]
    public async Task<ActionResult<Post>> GetByPostId(int PostId)
    {
        var Post = await _post.GetById(PostId);
        return Ok(Post);
    }






}