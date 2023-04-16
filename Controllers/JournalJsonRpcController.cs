using Journal.Models;
using Journal.Services;

namespace Journal.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/jsonrpc")]
public class JournalJsonRpcController : ControllerBase
{
    private readonly JsonRpcHandlerFactory _handlerFactory;

    public JournalJsonRpcController(JournalService journalService)
    {
        _handlerFactory = new JsonRpcHandlerFactory(journalService);
    }

    [HttpPost]
    public async Task<ActionResult> HandleJsonRpc([FromBody] JsonRpcRequest request)
    {
        if (request.JsonRpc != "2.0")
        {
            return BadRequest($"Unsupported jsonrpc version {request.JsonRpc}");
        }
        
        var handler = _handlerFactory.GetHandler(request.Method);
        JsonRpcResponse resp = await handler.Execute(request);
        return Ok(resp);
    }
}