namespace DjayStories.Core.Chats;

public class ChatService : IChatService
{
    private readonly ChatManager _chatManager;

    public ChatService(ChatManager chatManager)
    {
        _chatManager = chatManager;
    }

    public Task<List<Message>> GetMessagesAsync(int conversationId)
    {
        return _chatManager.GetMessagesAsync(conversationId, limit: 100);
    }

    public Task<Message> SendMessageAsync(int conversationId, Guid userId, Guid? receiverId, string content)
    {
        return _chatManager.SendMessageAsync(conversationId, userId, receiverId, content);
    }
}