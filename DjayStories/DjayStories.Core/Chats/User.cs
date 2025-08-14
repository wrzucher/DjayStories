namespace DjayStories.Core.Chats;
public class User
{
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();
}