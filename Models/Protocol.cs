using System.Text.Json;

namespace Journal.Models;

public class GetPreviewsRequest
{
    public MessageId FromId { get; set; } = null!;
    public int Count { get; set; }
}

public class GetPreviewsResponse
{
    public List<MessagePreview> Previews { get; set; } = null!;
}

public class GetContentRequest
{
    public MessageId Id { get; set; } = null!;
}

public class GetContentResponse
{
    public MessageContent Content { get; set; } = null!;
}

public class AddMessageRequest
{ 
    public string Header { get; set; } = null!;
    public JsonDocument Preview { get; set; } = JsonDocument.Parse("{}");
    public JsonDocument Content { get; set; } = JsonDocument.Parse("{}");
}

public class EmptyResponse
{
    
}

public class DeleteMessageRequest
{
    public MessageId Id { get; set; } = null!;
}

public class SetMessageSeenRequest
{
    public MessageId Id { get; set; } = null!;
}
