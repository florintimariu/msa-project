using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoSocialApp.Models;

[Table("todos")]
public class Todo
{
    [Key]
    [Column("todoid")]
    public int TodoId { get; set; }

    [Required]
    [Column("userid")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("due_date")]
    public DateTime? DueDate { get; set; }

    // Navigation property
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
