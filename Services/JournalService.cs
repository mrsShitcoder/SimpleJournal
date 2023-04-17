using Journal.Models;
namespace Journal.Services;

public class JournalService
{
    private readonly CacheByUser _usersCache;
    private readonly CacheByMessage _messagesCache;
    private readonly JournalDatabaseService _databaseService;

    public JournalService(JournalDatabaseService databaseService, CacheByUser usersCache,
        CacheByMessage messagesCache)
    {
        _databaseService = databaseService;
        _usersCache = usersCache;
        _messagesCache = messagesCache;
    }

    public async Task<List<Message>> GetMessagesList(MessageId fromId, int count)
    {
        var cachedData = _usersCache.GetList(fromId, count);
        if (cachedData != null)
        {
            return cachedData;
        }

        List<Message> messages = await _databaseService.GetMessageList(fromId, count);
        if (messages.Any())
        {
            _usersCache.AddOrUpdateMultiple(fromId.UserId, messages);
        }
        
        return messages;
    }

    public async Task<Message> GetOneMessage(MessageId messageId)
    {
        var cachedData = _messagesCache.Get(messageId);
        if (cachedData != null)
        {
            return cachedData;
        }

        var fetchedData = await _databaseService.GetMessage(messageId);

        if (fetchedData != null)
        {
            _messagesCache.AddOrUpdate(messageId, fetchedData);
            return fetchedData;
        }

        throw new KeyNotFoundException($"Not found messageId: {messageId} neither in cache nor in DB");
    }

    public async Task AddMessage(MessageId messageId, Message message)
    {
        await _databaseService.CreateMessage(message);
        _usersCache.AddOrUpdate(messageId, message);
        _messagesCache.AddOrUpdate(messageId, message);
    }

    public async Task DeleteMessage(MessageId messageId)
    {
        await _databaseService.DeleteMessage(messageId);
        _usersCache.Delete(messageId);
        _messagesCache.Delete(messageId);
    }

    public async Task SetMessageSeen(MessageId messageId)
    {
        await _databaseService.UpdateMessageState(messageId, MessageState.Seen);

        var message = _messagesCache.Get(messageId) ?? await _databaseService.GetMessage(messageId);
        if (message != null)
        {
            _messagesCache.AddOrUpdate(messageId, message);
        }
    }

    public async Task<MessageId> GetNewMessageId(ulong userId)
    {
        return await _databaseService.CreateNewMessageId(userId);
    }
}
