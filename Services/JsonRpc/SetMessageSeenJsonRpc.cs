using Journal.Models;
using Journal.Services.JsonRpc;

namespace Journal.Services;

public class SetMessageSeenJsonRpc : IJsonRpcHandler<SetMessageSeenRequest, EmptyResponse>
{
    private readonly JournalService _journalService;

    public SetMessageSeenJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<EmptyResponse> Execute(ulong userId, SetMessageSeenRequest request)
    {
        if (request.Id.UserId != userId)
        {
            throw new BadRequestException(
                $"You cannot modify other user's messages. UserId {userId}, MessageId.UserId {request.Id.UserId}");
        }
        await _journalService.SetMessageSeen(request.Id);
        return new EmptyResponse();
    }

    public string GetMethod()
    {
        return "SetMessageSeen";
    }
}
