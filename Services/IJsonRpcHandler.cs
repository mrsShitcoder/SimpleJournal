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
    public Task<TResponse> Execute(ulong userId, TRequest request);

    ulong GetUserId(JsonRpcRequest jsonRpcRequest)
    {
        return jsonRpcRequest.Params.RootElement.GetProperty("userId").GetUInt64();
    }
    
    TRequest PrepareRequest(JsonRpcRequest jsonRpcRequest)
    {
        var concreteRequest = jsonRpcRequest.Params.RootElement;
        var request = JsonSerializer.Deserialize<TRequest>(concreteRequest.GetProperty("request").GetRawText());
        if (request == null)
        {
            throw new ArgumentException($"Request {concreteRequest} is not parseable");
        }

        return request;
    }

    JsonRpcResponse PrepareResponse(TResponse response, string id)
    {
        var jsonResponse = new JsonRpcResponse();
        jsonResponse.Id = id;
        jsonResponse.Result = JsonSerializer.SerializeToDocument(new { Response = response });
        return jsonResponse;
    }

    async Task<JsonRpcResponse> IJsonRpcHandlerBase.Execute(JsonRpcRequest request) 
    {
        ulong userId = GetUserId(request);
        if (userId == 0)
        {
            throw new Exception("Got zero userId");
        }
        TRequest req = PrepareRequest(request);
        Console.WriteLine($"Got request: {JsonSerializer.Serialize(req)}");
        TResponse resp = await Execute(userId, req);
        return PrepareResponse(resp, request.Id);
    }
}
