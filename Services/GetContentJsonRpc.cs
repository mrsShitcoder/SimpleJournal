using Journal.Models;

namespace Journal.Services;

public class GetContentJsonRpc  : IJsonRpcHandler<GetContentRequest, GetContentResponse>
{
    private JournalService _journalService;
    public GetContentJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<GetContentResponse> Execute(ulong userId, GetContentRequest request)
    {
        if (request.Id.UserId != userId)
        {
            throw new Exception(
                $"You cannot see other user's messages. UserId {userId}, MessageId.UserId {request.Id.UserId}");
        }
        var response = new GetContentResponse();
        Message message = await _journalService.GetOneMessage(request.Id);
        response.Content = new MessageContent(message);
        return response;
    }

    public string GetMethod()
    {
        return "GetContent";
    }

}