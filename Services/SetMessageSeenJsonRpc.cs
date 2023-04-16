using Journal.Models;

namespace Journal.Services;

public class SetMessageSeenJsonRpc : IJsonRpcHandler<SetMessageSeenRequest, NullResponse>
{
    private readonly JournalService _journalService;

    public SetMessageSeenJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<NullResponse> Execute(SetMessageSeenRequest request)
    {
        await _journalService.SetMessageSeen(request.Id);
        return new NullResponse();
    }

    public string GetMethod()
    {
        return "SetMessageSeen";
    }
}
