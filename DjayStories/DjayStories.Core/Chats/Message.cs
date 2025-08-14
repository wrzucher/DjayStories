namespace DjayStories.Core.Chats;
public class Message
{
    public int MessageId { get; set; }

    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
    
    public Guid SenderId { get; set; }
    public Guid? ReceiverId { get; set; }
    public User Sender { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public string? Action { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsEdited { get; set; } = false;

    public ICollection<MessageStatus> Statuses { get; set; } = new List<MessageStatus>();
}