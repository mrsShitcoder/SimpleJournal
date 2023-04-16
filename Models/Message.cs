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
    [BsonId] public MessageId Id { get; set; } = null!;
    
    public JsonDocument Content { get; set; } = null!;
}

public class MessagePreview
{
    [BsonId] public MessageId Id { get; set; } = null!;
    
    public DateTime Date { get; set; }

    public string Header { get; set; } = null!;

    public JsonDocument Preview { get; set; } = null!;
    
    public MessageState State { get; set; }
}

