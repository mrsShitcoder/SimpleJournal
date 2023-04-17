using Journal.Models;
using MongoDB.Bson;

namespace Journal.Services;

public class AddMessageJsonRpc : IJsonRpcHandler<AddMessageRequest, EmptyResponse>
{
    private readonly JournalService _journalService;

    public AddMessageJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<EmptyResponse> Execute(ulong userId, AddMessageRequest request)
    {
        MessageId messageId = await _journalService.GetNewMessageId(userId);
        
        var message = new Message
        {
            Id = messageId, 
            Date = DateTime.Now, 
            Header = request.Header, 
            Preview = BsonDocument.Parse(request.Preview.RootElement.GetRawText()),
            Content = BsonDocument.Parse(request.Content.RootElement.GetRawText()), 
            State = MessageState.Unseen
        };

        await _journalService.AddMessage(messageId, message);
        return new EmptyResponse();
    }

    public string GetMethod()
    {
        return "AddMessage";
    }
}
