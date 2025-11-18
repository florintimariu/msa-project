using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoSocialApp.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("userid")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Todo> Todos { get; set; } = new List<Todo>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Friend> Following { get; set; } = new List<Friend>();
    public ICollection<Friend> Followers { get; set; } = new List<Friend>();
}
