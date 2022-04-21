using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Twitter_BE.DTOs;
using Twitter_BE.Models;
using Twitter_BE.Repositories;
using Twitter_BE.Utilities;

namespace Twitter_BE.Controllers;

[ApiController]
[Route("api/user")]

public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _user;
    private readonly IConfiguration _config;


    public UserController(ILogger<UserController> logger, IUserRepository user, IConfiguration config)
    {
        _logger = logger;
        _user = user;
        _config = config;
    }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == UserConstants.Id).First().Value);
    }

    [HttpPost("Register")]

    public async Task<ActionResult<User>> CreateUser([FromBody] UserDTO Data)
    {
        var toCreateUser = new User
        {
            Name = Data.Name.Trim().ToLower(),
            Email = Data.Email.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(Data.Password)
        };
        var createdUser = await _user.Create(toCreateUser);
        return StatusCode(201, createdUser);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> UpdateUser(
    [FromBody] UserUpdateDTO Data)
    {
        var User_Id = GetUserIdFromClaims(User.Claims);

        var existing = await _user.GetById(User_Id);
        if (existing is null)
            return NotFound("No user found with given id");

        if (existing.Id != User_Id)
            return StatusCode(403, "You cannot update others Name");


        var toUpdateUser = existing with
        {
            Name = Data.Name?.Trim() ?? existing.Name,
        };
        await _user.Update(toUpdateUser);
        return NoContent();
    }


    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResDTO>> Login(
        [FromBody] UserLoginDTO Data)
    {
        if (!IsValidEmailAddress(Data.Email))
            return Unauthorized("Incorrect Email");
        var existingUser = await _user.GetUser(Data.Email);

        if (existingUser is null)
            return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(Data.Password, existingUser.Password))
            return Unauthorized("Incorrect Password");



        var token = Generate(existingUser);

        var res = new UserLoginResDTO
        {
            Id = existingUser.Id,
            Name = existingUser.Name,
            Email = existingUser.Email,
            Token = token
        };

        return Ok(res);
    }

    private string Generate(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(UserConstants.Id, user.Id.ToString()),
            new Claim(UserConstants.Name, user.Name),
            new Claim(UserConstants.Email, user.Email),
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }


    private bool IsValidEmailAddress(string email)
    {
        try
        {
            var emailChecked = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

}
