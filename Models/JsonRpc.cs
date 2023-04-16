using System.Text.Json;

namespace Journal.Models;

public class JsonRpcRequest
{
    public string JsonRpc { get; set; } = null!;
    public string Method { get; set; } = null!;
    public JsonDocument Params { get; set; } = null!;
    public string Id { get; set; } = null!;
}

public class JsonRpcResponse
{
    public string JsonRpc { get; set; } = "2.0";
    public JsonDocument Result { get; set; } = null!;
    public string Id { get; set; } = null!;
}
