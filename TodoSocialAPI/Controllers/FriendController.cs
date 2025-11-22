using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoSocialApp.Data;
using TodoSocialApp.DTOs;
using TodoSocialApp.Models;

namespace TodoSocialApp.Controllers;

[ApiController]
[Route("api")]
public class FriendController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<FriendController> _logger;

    public FriendController(AppDbContext context, ILogger<FriendController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("/api/friend")]
    public async Task<ActionResult> AddFriend([FromQuery] int userId, [FromBody] AddFollowRequest request)
    {
        try
        {
            if (userId <= 0 || request.FollowedUserId <= 0)
            {
                return BadRequest(new { message = "Valid userId and followedUserId are required." });
            }

            if (userId == request.FollowedUserId)
            {
                return BadRequest(new { message = "You cannot follow yourself." });
            }

            // Check if both users exist
            var user = await _context.Users.FindAsync(userId);
            var followedUser = await _context.Users.FindAsync(request.FollowedUserId);

            if (user == null || followedUser == null)
            {
                return NotFound(new { message = "One or both users not found." });
            }

            // Check if friendship already exists
            var existingFriend = await _context.Friends
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FollowedUserId == request.FollowedUserId);

            if (existingFriend != null)
            {
                return Conflict(new { message = "You are already following this user." });
            }

            var friend = new Friend
            {
                UserId = userId,
                FollowedUserId = request.FollowedUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Friend added successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding friend");
            return StatusCode(500, new { message = "An error occurred while adding the friend." });
        }
    }

    [HttpDelete("/api/friend/")]
    public async Task<ActionResult> RemoveFriend([FromQuery] int userId, [FromQuery] int followedUserId)
    {
        try
        {
            if (userId <= 0 || followedUserId <= 0)
            {
                return BadRequest(new { message = "Valid userId and followedUserId are required." });
            }

            var friend = await _context.Friends
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FollowedUserId == followedUserId);

            if (friend == null)
            {
                return NotFound(new { message = "Friendship not found." });
            }

            _context.Friends.Remove(friend);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Friend removed successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing friend");
            return StatusCode(500, new { message = "An error occurred while removing the friend." });
        }
    }

    [HttpGet("/api/friend/followed")]
    public async Task<ActionResult<IEnumerable<FriendResponse>>> GetFriends([FromQuery] int userId)
    {
        try
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Valid userId is required." });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var friends = await _context.Friends
                .Where(f => f.UserId == userId)
                .Include(f => f.FollowedUser)
                .Select(f => new FriendResponse
                {
                    UserId = f.FollowedUser.UserId,
                    Username = f.FollowedUser.Username,
                    Email = f.FollowedUser.Email,
                    FollowedAt = f.CreatedAt
                })
                .ToListAsync();

            return Ok(friends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting friends");
            return StatusCode(500, new { message = "An error occurred while retrieving friends." });
        }
    }

    [HttpGet("/api/friend/following")]
    public async Task<ActionResult<IEnumerable<FriendResponse>>> GetFollowers([FromQuery] int userId)
    {
        try
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Valid userId is required." });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var followers = await _context.Friends
                .Where(f => f.FollowedUserId == userId)
                .Include(f => f.User)
                .Select(f => new FriendResponse
                {
                    UserId = f.User.UserId,
                    Username = f.User.Username,
                    Email = f.User.Email,
                    FollowedAt = f.CreatedAt
                })
                .ToListAsync();

            return Ok(followers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers");
            return StatusCode(500, new { message = "An error occurred while retrieving followers." });
        }
    }
}
