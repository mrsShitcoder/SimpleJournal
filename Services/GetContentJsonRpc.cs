using Journal.Models;

namespace Journal.Services;

public class GetContentJsonRpc  : IJsonRpcHandler<GetContentRequest, GetContentResponse>
{
    private JournalService _journalService;
    public GetContentJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<GetContentResponse> Execute(GetContentRequest request)
    {
        var response = new GetContentResponse();
        response.Content = await _journalService.GetContent(request.Id);
        return response;
    }

    public string GetMethod()
    {
        return "GetContent";
    }

}