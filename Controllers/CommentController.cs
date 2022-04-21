using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twitter_BE.DTOs;
using Twitter_BE.Models;
using Twitter_BE.Repositories;
using Twitter_BE.Utilities;

namespace Twitter_BE.Controllers;

[ApiController]
[Route("api/comment")]
[Authorize]

public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentRepository _comment;

    public CommentController(ILogger<CommentController> logger, ICommentRepository comment)
    {
        _logger = logger;
        _comment = comment;
    }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == UserConstants.Id).First().Value);
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> CreateComment([FromQuery] int post_id,
        [FromBody] CommentCreateDTO Data)
    {
        var User_Id = GetUserIdFromClaims(User.Claims);

        var toCreateComment = new Comment
        {
            Text = Data.Text.Trim(),
            UserId = User_Id,
            PostId = post_id,
        };
        var createdComment = await _comment.Create(toCreateComment);
        return StatusCode(201, createdComment);
    }

    [HttpGet]
    public async Task<ActionResult<List<Comment>>> GetAllComments([FromQuery] int post_id)
    {
        var allComment = await _comment.GetAll(post_id);
        return Ok(allComment);
    }

    [HttpDelete("{comment_id}")]
    public async Task<ActionResult> DeleteComment(int comment_id)
    {
        var User_Id = GetUserIdFromClaims(User.Claims);


        var existingItem = await _comment.GetById(comment_id);
        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != User_Id)
            return StatusCode(403, "You cannot delete others comment");

        await _comment.Delete(comment_id);
        return NoContent();
    }


}