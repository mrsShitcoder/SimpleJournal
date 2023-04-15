using Journal.Models;
namespace Journal.Services;

public class JournalService
{
    private readonly PreviewsCache _previewsCache;
    private readonly ContentsCache _contentsCache;
    private readonly JournalDatabaseService _databaseService;

    public JournalService(JournalDatabaseService databaseService, PreviewsCache previewsCache,
        ContentsCache contentsCache)
    {
        _databaseService = databaseService;
        _previewsCache = previewsCache;
        _contentsCache = contentsCache;
    }

    public async Task<List<MessagePreview>> GetPreviews(MessageId fromId, int count)
    {
        var cachedData = _previewsCache.Get(fromId, count);
        if (cachedData != null)
        {
            return cachedData;
        }
        return await _databaseService.GetMessagePreviewsAsync(fromId, count);
    }

    public async Task<MessageContent> GetContent(MessageId messageId)
    {
        var cachedContent = _contentsCache.Get(messageId);
        if (cachedContent != null)
        {
            return cachedContent;
        }

        var fetchedData = await _databaseService.GetMessageContentAsync(messageId);

        if (fetchedData != null)
        {
            return fetchedData;
        }

        throw new KeyNotFoundException($"Not found messageId: {messageId} neither in cache nor in DB");
    }

    public async Task AddMessage(MessageId messageId, MessagePreview preview, MessageContent content)
    {
        await _databaseService.CreateMessage(preview, content);
        _previewsCache.AddOrUpdate(messageId, preview);
        _contentsCache.AddOrUpdate(messageId, content);
    }

    public async Task DeleteMessage(MessageId messageId)
    {
        await _databaseService.DeleteMessage(messageId);
        _previewsCache.Delete(messageId);
        _contentsCache.Delete(messageId);
    }

    public async Task<UserSequence> GetNewUserSequence(ulong userId)
    {
        var lastSeq = await _databaseService.GetUserSequenceAsync(userId) ?? new UserSequence();
        lastSeq.UserId = userId;
        lastSeq.MaxSequence++;
        await _databaseService.AddUserSequenceAsync(lastSeq);
        return lastSeq;
    }
}
