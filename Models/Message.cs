using System.Text.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace Journal.Models;

public enum MessageState
{
    Unseen = 1,
    Seen = 2
}

public class MessageContent
{
    public MessageContent(Message message)
    {
        Id = message.Id;
        Date = message.Date;
        Header = message.Header;
        Content = message.Content;
    }
    public MessageId Id { get; set; } = null!;
    
    public DateTime Date { get; set; }

    public string Header { get; set; } = null!;
    
    public JsonDocument Content { get; set; } = null!;
}

public class MessagePreview
{
    public MessagePreview(Message message)
    {
        Id = message.Id;
        Date = message.Date;
        Header = message.Header;
        Preview = message.Preview;
        State = message.State;
    }
    
    public MessageId Id { get; set; } = null!;
    
    public DateTime Date { get; set; }

    public string Header { get; set; } = null!;

    public JsonDocument Preview { get; set; } = null!;
    
    public MessageState State { get; set; }
}

public class Message
{
    [BsonId] public MessageId Id { get; set; } = null!;
    
    public DateTime Date { get; set; }

    public string Header { get; set; } = null!;

    public JsonDocument Preview { get; set; } = null!;
    
    public JsonDocument Content { get; set; } = null!;
    
    public MessageState State { get; set; }
    
}

