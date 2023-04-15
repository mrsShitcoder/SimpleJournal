using System.Text.Json;
using Journal.Models;
using Journal.Services;

namespace Journal.Controllers;
using Microsoft.AspNetCore.Mvc;

//TODO do it with jsonrpc
[ApiController]
[Route("api/[controller]")]
public class JournalController : ControllerBase
{
    private readonly JournalService _journalService;


    public JournalController(JournalService journalService)
    {
        _journalService = journalService;
    }

    [HttpGet("previews")]
    public async Task<ActionResult<List<MessagePreview>>> GetPreviews(ulong userId, [FromBody] ulong seq,
        [FromBody] int count)
    {
        var messageId = new MessageId(userId, seq);
        return await _journalService.GetPreviews(messageId, count);
    }
    
    [HttpGet("contents")]
    public async Task<ActionResult<MessageContent>> GetContent(ulong userId, [FromBody] ulong seq)
    {
        var messageId = new MessageId(userId, seq);
        return await _journalService.GetContent(messageId);
    }

    [HttpPost]
    public async Task<IActionResult> AddMessage(ulong userId, [FromBody] string header,
        [FromBody] JsonDocument previewBody,
        [FromBody] JsonDocument contentBody)
    {
        var newSeq = await _journalService.GetNewUserSequence(userId);
        var messageId = new MessageId(userId, newSeq.MaxSequence);
        var preview = new MessagePreview();
        preview.Id = messageId;
        preview.Date = DateTime.Now;
        preview.Header = header;
        preview.Preview = previewBody;
        preview.State = MessageState.Unseen;

        var content = new MessageContent();
        content.Id = messageId;
        content.Content = contentBody;
        
        await _journalService.AddMessage(messageId, preview, content);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMessage(ulong userId, [FromBody] ulong seq)
    {
        var messageId = new MessageId(userId, seq);
        await _journalService.DeleteMessage(messageId);
        return Ok();
    }
}