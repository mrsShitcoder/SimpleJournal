using Journal.Models;

namespace Journal.Services;

public class DeleteMessageJsonRpc : IJsonRpcHandler<DeleteMessageRequest, EmptyResponse>
{
    private readonly JournalService _journalService;

    public DeleteMessageJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<EmptyResponse> Execute(ulong userId, DeleteMessageRequest request)
    {
        if (request.Id.UserId != userId)
        {
            throw new Exception(
                $"You cannot delete something that is not yours. UserId {userId}, MessageId.UserId {request.Id.UserId}");
        }
        
        await _journalService.DeleteMessage(request.Id);
        return new EmptyResponse();
    }

    public string GetMethod()
    {
        return "DeleteMessage";
    }
}
