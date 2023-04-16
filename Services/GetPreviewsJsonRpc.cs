using Journal.Models;

namespace Journal.Services;

public class GetPreviewsJsonRpc : IJsonRpcHandler<GetPreviewsRequest, GetPreviewsResponse>
{
    private JournalService _journalService;
    public GetPreviewsJsonRpc(JournalService journalService)
    {
        _journalService = journalService;
    }
    
    public async Task<GetPreviewsResponse> Execute(GetPreviewsRequest request)
    {
        var response = new GetPreviewsResponse();
        response.Previews = await _journalService.GetPreviews(request.FromId, request.Count);
        return response;
    }

    public string GetMethod()
    {
        return "GetPreviews";
    }
}
