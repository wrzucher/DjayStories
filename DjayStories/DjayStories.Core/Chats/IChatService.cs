namespace DjayStories.Core.Chats;

public interface IChatService
{
    Task<List<Message>> GetMessagesAsync(int conversationId);
    Task<Message> SendMessageAsync(int conversationId, Guid userId, Guid? receiverId, string content);
}