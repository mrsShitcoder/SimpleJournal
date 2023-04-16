using System.Text.Json;
using Journal.Models;

namespace Journal.Services;

public interface IJsonRpcHandlerBase
{
    string GetMethod();
    
    Task<JsonRpcResponse> Execute(JsonRpcRequest request);
}

public interface IJsonRpcHandler<TRequest, TResponse> : IJsonRpcHandlerBase
{
    public Task<TResponse> Execute(TRequest request);

    public TRequest PrepareRequest(JsonRpcRequest jsonRpcRequest)
    {
        var concreteRequest = jsonRpcRequest.Params.RootElement;
        var request = JsonSerializer.Deserialize<TRequest>(concreteRequest.GetProperty("request").GetRawText());
        if (request == null)
        {
            throw new ArgumentException($"Request {concreteRequest} is not parseable");
        }

        return request;
    }

    public JsonRpcResponse PrepareResponse(TResponse response, string id)
    {
        var jsonResponse = new JsonRpcResponse();
        jsonResponse.Id = id;
        jsonResponse.Result = JsonSerializer.SerializeToDocument(new { Response = response });
        return jsonResponse;
    }

    async Task<JsonRpcResponse> IJsonRpcHandlerBase.Execute(JsonRpcRequest request) 
    {
        TRequest req = PrepareRequest(request);
        TResponse resp = await Execute(req);
        return PrepareResponse(resp, request.Id);
    }
}
