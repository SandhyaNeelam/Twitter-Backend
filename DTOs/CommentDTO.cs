using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Twitter_BE.DTOs;

public record CommentCreateDTO
{
    [JsonPropertyName("text")]
    [Required]
    [MinLength(4)]
    [MaxLength(60)]
    public string Text { get; set; }

    // [JsonPropertyName("user_id")]
    // public int UserId { get; set; }

    // [JsonPropertyName("post_id")]
    // public int PostId { get; set; }
}

