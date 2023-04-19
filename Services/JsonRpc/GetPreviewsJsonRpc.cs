using Journal.Models;

namespace Journal.Services.JsonRpc;

public class GetPreviewsJsonRpc : IJsonRpcHandler<GetPreviewsRequest, GetPreviewsResponse>
{
    private JournalService _journalService;
    public GetPreviewsJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }
    
    public async Task<GetPreviewsResponse> Execute(ulong userId, GetPreviewsRequest request)
    {
        if (request.FromId.UserId != userId)
        {
            throw new BadRequestException(
                $"You cannot see other user's messages. UserId {userId}, MessageId.UserId {request.FromId.UserId}");
        }
        List<Message> messages = await _journalService.GetMessagesList(request.FromId, request.Count);
        var response = new GetPreviewsResponse{Previews = new List<MessagePreview>()};

        foreach (var message in messages)
        {
            response.Previews.Add(new MessagePreview(message));
        }
        
        return response;
    }

    public string GetMethod()
    {
        return "GetPreviews";
    }
}
