using Microsoft.EntityFrameworkCore;

namespace DjayStories.Core.Chats;

public class ChatManager
{
    private readonly DbContextOptions<ChatContext> _contextOptions;

    public ChatManager(DbContextOptions<ChatContext> contextOptions)
    {
        _contextOptions = contextOptions;
    }

    // Events
    public event EventHandler<MessageEventArgs>? MessageSent;
    public event EventHandler<ConversationEventArgs>? ConversationCreated;

    // Create a new conversation (1-on-1 or group)
    public async Task<Conversation> CreateConversationAsync(IEnumerable<Guid> userIds, string? title = null, bool isGroup = false)
    {
        var conversation = new Conversation
        {
            Title = title,
            IsGroup = isGroup,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var userId in userIds.Distinct())
        {
            conversation.Participants.Add(new ConversationParticipant
            {
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
                Role = "member"
            });
        }

        using var context = new ChatContext(_contextOptions);

        context.Conversations.Add(conversation);
        await context.SaveChangesAsync();

        ConversationCreated?.Invoke(this, new ConversationEventArgs(conversation));

        return conversation;
    }

    // Send a message to a conversation
    public async Task<Message> SendMessageAsync(int conversationId, Guid senderId, Guid? receiverId, string content, string? action = null)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            ReceiverId = receiverId,
            Action = action,
            SentAt = DateTime.UtcNow,
        };

        using var context = new ChatContext(_contextOptions);
        context.Messages.Add(message);

        // Optional: Create unread status for all participants
        var participantUserIds = await context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId && p.UserId != senderId)
            .Select(p => p.UserId)
            .ToListAsync();

        foreach (var userId in participantUserIds)
        {
            context.MessageStatuses.Add(new MessageStatus
            {
                Message = message,
                UserId = userId,
                IsRead = false
            });
        }

        await context.SaveChangesAsync();

        MessageSent?.Invoke(this, new MessageEventArgs(message));

        return message;
    }

    // Mark a message as read
    public async Task MarkMessageAsReadAsync(int messageId, Guid userId)
    {
        using var context = new ChatContext(_contextOptions);
        var status = await context.MessageStatuses
            .FirstOrDefaultAsync(ms => ms.MessageId == messageId && ms.UserId == userId);

        if (status != null && !status.IsRead)
        {
            status.IsRead = true;
            status.ReadAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    // Get messages for a conversation
    public async Task<List<Message>> GetMessagesAsync(int conversationId, int limit = 50)
    {
        using var context = new ChatContext(_contextOptions);
        return await context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Take(limit)
            .Include(m => m.Sender)
            .ToListAsync();
    }

    // Add a user to an existing conversation
    public async Task AddUserToConversationAsync(int conversationId, Guid userId, string role = "member")
    {
        using var context = new ChatContext(_contextOptions);
        var exists = await context.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

        if (!exists)
        {
            context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversationId,
                UserId = userId,
                Role = role,
                JoinedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }
    }
}
