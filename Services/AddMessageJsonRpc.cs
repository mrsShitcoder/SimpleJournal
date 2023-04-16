using Journal.Models;

namespace Journal.Services;

public class AddMessageJsonRpc : IJsonRpcHandler<AddMessageRequest, EmptyResponse>
{
    private readonly JournalService _journalService;

    public AddMessageJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<EmptyResponse> Execute(AddMessageRequest request)
    {
        UserSequence seq = await _journalService.GetNewUserSequence(request.UserId);
        var messageId = new MessageId(seq);

        var preview = new MessagePreview
        {
            Id = messageId,
            Date = DateTime.Now,
            Header = request.Header,
            Preview = request.Preview,
            State = MessageState.Unseen
        };


        var content = new MessageContent { Id = messageId, Content = request.Content };
        await _journalService.AddMessage(messageId, preview, content);
        return new EmptyResponse();
    }

    public string GetMethod()
    {
        return "AddMessage";
    }
}
