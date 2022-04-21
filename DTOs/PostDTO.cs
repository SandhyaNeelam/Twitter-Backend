using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Twitter_BE.DTOs;

public record PostDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}

public record PostCreateDTO
{
    [JsonPropertyName("title")]
    [Required]
    [MinLength(4)]
    [MaxLength(90)]
    public string Title { get; set; }

    // [JsonPropertyName("user_id")]
    // public int UserId { get; set; }
}

public record PostUpdateDTO
{
    [JsonPropertyName("title")]
    [MinLength(4)]
    [MaxLength(255)]
    public string Title { get; set; } = null;

    // [JsonPropertyName("updated_at")]
    // public DateTimeOffset UpdatedAt { get; set; }
}