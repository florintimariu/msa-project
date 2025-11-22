using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoSocialApp.Data;
using TodoSocialApp.DTOs;
using TodoSocialApp.Models;

namespace TodoSocialApp.Controllers;

[ApiController]
[Route("api")]
public class MessageController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<MessageController> _logger;

    public MessageController(AppDbContext context, ILogger<MessageController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("/api/message")]
    public async Task<ActionResult<MessageResponse>> SendMessage([FromQuery] int senderId, [FromBody] CreateMessageRequest request)
    {
        try
        {
            if (senderId <= 0 || request.ReceiverId <= 0)
            {
                return BadRequest(new { message = "Valid senderId and receiverId are required." });
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { message = "Message content is required." });
            }

            // Check if both users exist
            var sender = await _context.Users.FindAsync(senderId);
            var receiver = await _context.Users.FindAsync(request.ReceiverId);

            if (sender == null || receiver == null)
            {
                return NotFound(new { message = "Sender or receiver not found." });
            }

            var message = new Message
            {
                SenderId = senderId,
                MessageType = request.MessageType,
                Content = request.Content,
                MediaUrl = request.MediaUrl,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var response = new MessageResponse
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                SenderUsername = sender.Username,
                MessageType = message.MessageType,
                Content = message.Content,
                MediaUrl = message.MediaUrl,
                SentAt = message.SentAt,
                IsRead = message.IsRead
            };

            return CreatedAtAction(nameof(GetMessage), new { messageId = message.MessageId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(500, new { message = "An error occurred while sending the message." });
        }
    }

    [HttpGet("/api/message")]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetMessages([FromQuery] int userId)
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

            // Get messages sent by the user
            var messages = await _context.Messages
                .Where(m => m.SenderId == userId)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.SentAt)
                .Select(m => new MessageResponse
                {
                    MessageId = m.MessageId,
                    SenderId = m.SenderId,
                    SenderUsername = m.Sender.Username,
                    MessageType = m.MessageType,
                    Content = m.Content,
                    MediaUrl = m.MediaUrl,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                })
                .ToListAsync();

            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting messages");
            return StatusCode(500, new { message = "An error occurred while retrieving messages." });
        }
    }

    [HttpGet("/api/messages/{messageId}")]
    public async Task<ActionResult<MessageResponse>> GetMessage(int messageId)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.MessageId == messageId);

        if (message == null)
        {
            return NotFound(new { message = "Message not found." });
        }

        var response = new MessageResponse
        {
            MessageId = message.MessageId,
            SenderId = message.SenderId,
            SenderUsername = message.Sender.Username,
            MessageType = message.MessageType,
            Content = message.Content,
            MediaUrl = message.MediaUrl,
            SentAt = message.SentAt,
            IsRead = message.IsRead
        };

        return Ok(response);
    }

    [HttpPut("/api/message/read")]
    public async Task<ActionResult> MarkMessageAsRead([FromQuery] int messageId)
    {
        try
        {
            if (messageId <= 0)
            {
                return BadRequest(new { message = "Valid messageId is required." });
            }

            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return NotFound(new { message = "Message not found." });
            }

            message.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Message marked as read." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as read");
            return StatusCode(500, new { message = "An error occurred while marking the message as read." });
        }
    }
}
