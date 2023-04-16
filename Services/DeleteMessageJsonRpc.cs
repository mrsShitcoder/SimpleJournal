using Journal.Models;

namespace Journal.Services;

public class DeleteMessageJsonRpc : IJsonRpcHandler<DeleteMessageRequest, NullResponse>
{
    private readonly JournalService _journalService;

    public DeleteMessageJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<NullResponse> Execute(DeleteMessageRequest request)
    {
        await _journalService.DeleteMessage(request.Id);
        return new NullResponse();
    }

    public string GetMethod()
    {
        return "DeleteMessage";
    }
}
