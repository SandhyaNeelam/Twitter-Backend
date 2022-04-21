using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Twitter_BE.DTOs;

public record UserLoginDTO
{
    [JsonPropertyName("email")]
    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    [Required]
    public string Password { get; set; }
}


public record UserLoginResDTO
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}

public record UserUpdateDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}


public record UserDTO
{

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}