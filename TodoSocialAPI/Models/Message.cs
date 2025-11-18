using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoSocialApp.Models;

[Table("messages")]
public class Message
{
    [Key]
    [Column("messageid")]
    public int MessageId { get; set; }

    [Required]
    [Column("senderid")]
    public int SenderId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("message_type")]
    public string MessageType { get; set; } = "text"; // text, image, etc.

    [Required]
    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("media_url")]
    public string? MediaUrl { get; set; }

    [Column("sent_at")]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    // Navigation property
    [ForeignKey("SenderId")]
    public User Sender { get; set; } = null!;
}
