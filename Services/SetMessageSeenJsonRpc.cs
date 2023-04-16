using Journal.Models;

namespace Journal.Services;

public class SetMessageSeenJsonRpc : IJsonRpcHandler<SetMessageSeenRequest, EmptyResponse>
{
    private readonly JournalService _journalService;

    public SetMessageSeenJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<EmptyResponse> Execute(SetMessageSeenRequest request)
    {
        await _journalService.SetMessageSeen(request.Id);
        return new EmptyResponse();
    }

    public string GetMethod()
    {
        return "SetMessageSeen";
    }
}
