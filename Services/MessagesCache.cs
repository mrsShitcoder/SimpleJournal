using Microsoft.Extensions.Caching.Memory;
using Journal.Models;

namespace Journal.Services;

public class CacheByUser
{
    private readonly TimeSpan _cacheDuration;
    private readonly MemoryCache _messagesCache;

    public CacheByUser(int sizeLimit, TimeSpan cacheDuration)
    {
        _messagesCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = sizeLimit });
        _cacheDuration = cacheDuration;
    }

    public void AddOrUpdate(MessageId messageId, Message message)
    {
        var userMessages = _messagesCache.Get<Dictionary<MessageId, Message>>(messageId.UserId)
                           ?? new Dictionary<MessageId, Message>();
        userMessages[messageId] = message;
        var entryOptions = new MemoryCacheEntryOptions
            { SlidingExpiration = _cacheDuration, Size = userMessages.Count };
        _messagesCache.Set(messageId.UserId, userMessages, entryOptions);
    }

    public void AddOrUpdateMultiple(ulong userId, List<Message> messages)
    {
        var userMessages = _messagesCache.Get<Dictionary<MessageId, Message>>(userId)
                           ?? new Dictionary<MessageId, Message>();
        foreach (var message in messages)
        {
            userMessages[message.Id] = message;
        }

        var entryOptions = new MemoryCacheEntryOptions
            { SlidingExpiration = _cacheDuration, Size = userMessages.Count };
        _messagesCache.Set(userId, userMessages, entryOptions);
    }

    public List<Message>? GetList(MessageId fromId, int count)
    {
        var messages = _messagesCache.Get<Dictionary<MessageId, Message>>(fromId.UserId);
        if (messages == null)
        {
            return null;
        }

        var result = new List<Message>();
        int lastIndex = 0;

        foreach (var message in messages)
        {
            if (message.Key.CompareTo(fromId) > 0)
            {
                result.Add(message.Value);
                lastIndex++;
                if (lastIndex == count)
                {
                    break;
                }
            }
        }

        if (result.Count < count)
        {
            return null;
        }

        return result;
    }

    public void Delete(MessageId messageId)
    {
        var userMessages = _messagesCache.Get<Dictionary<MessageId, Message>>(messageId.UserId);
        if (userMessages == null)
        {
            return;
        }

        userMessages.Remove(messageId);
        if (!userMessages.Any())
        {
            _messagesCache.Remove(messageId.UserId);
        }
    }
}

public class CacheByMessage
{
    private readonly TimeSpan _cacheDuration;
    private readonly MemoryCache _messagesCache;

    public CacheByMessage(int sizeLimit, TimeSpan cacheDuration)
    {
        _messagesCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = sizeLimit });
        _cacheDuration = cacheDuration;
    }

    public void AddOrUpdate(MessageId messageId, Message message)
    {
        var entryOptions = new MemoryCacheEntryOptions { SlidingExpiration = _cacheDuration, Size = 1 };
        _messagesCache.Set(messageId, message, entryOptions);
    }

    public Message? Get(MessageId messageId)
    {
        return _messagesCache.Get<Message>(messageId);
    }

    public void Delete(MessageId messageId)
    {
        _messagesCache.Remove(messageId);
    }
}
