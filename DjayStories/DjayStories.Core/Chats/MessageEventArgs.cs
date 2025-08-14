namespace DjayStories.Core.Chats;
public class MessageEventArgs : EventArgs
{
    public Message Message { get; set; }

    public MessageEventArgs(Message message)
    {
        Message = message;
    }
}