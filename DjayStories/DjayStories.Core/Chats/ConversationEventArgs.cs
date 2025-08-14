namespace DjayStories.Core.Chats;
public class ConversationEventArgs : EventArgs
{
    public Conversation Conversation { get; set; }

    public ConversationEventArgs(Conversation conversation)
    {
        Conversation = conversation;
    }
}