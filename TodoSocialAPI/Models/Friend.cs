using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoSocialApp.Models;

[Table("friends")]
public class Friend
{
    [Column("userid")]
    public int UserId { get; set; }

    [Column("followeduserid")]
    public int FollowedUserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("FollowedUserId")]
    public User FollowedUser { get; set; } = null!;
}
