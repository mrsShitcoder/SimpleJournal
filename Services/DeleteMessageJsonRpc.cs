using Journal.Models;

namespace Journal.Services;

public class DeleteMessageJsonRpc : IJsonRpcHandler<DeleteMessageRequest, EmptyResponse>
{
    private readonly JournalService _journalService;

    public DeleteMessageJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<EmptyResponse> Execute(DeleteMessageRequest request)
    {
        await _journalService.DeleteMessage(request.Id);
        return new EmptyResponse();
    }

    public string GetMethod()
    {
        return "DeleteMessage";
    }
}
