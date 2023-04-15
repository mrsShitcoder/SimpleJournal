using Microsoft.Extensions.Caching.Memory;
using Journal.Models;

namespace Journal.Services;

public class PreviewsCache
{
    private readonly TimeSpan _cacheDuration;
    private readonly MemoryCache _previewsCache;

    public PreviewsCache(int sizeLimit, TimeSpan cacheDuration)
    {
        _previewsCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = sizeLimit });
        _cacheDuration = cacheDuration;
    }

    public void AddOrUpdate(MessageId messageId, MessagePreview preview)
    {
        var userMessages = _previewsCache.Get<Dictionary<MessageId, MessagePreview>>(messageId.UserId)
                           ?? new Dictionary<MessageId, MessagePreview>();
        userMessages[messageId] = preview;
        _previewsCache.Set(messageId.UserId, userMessages, DateTimeOffset.UtcNow.Add(_cacheDuration));
    }

    public List<MessagePreview>? Get(MessageId fromId, int count)
    {
        var previews = _previewsCache.Get<Dictionary<MessageId, MessagePreview>>(fromId.UserId);
        if (previews == null || !previews.ContainsKey(fromId))
        {
            return null;
        }

        var result = new List<MessagePreview>();
        int lastIndex = 0;

        foreach (var messageId in previews.Keys)
        {
            if (messageId.CompareTo(fromId) > 0)
            {
                result.Add(previews[messageId]);
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
        var userMessages = _previewsCache.Get<Dictionary<MessageId, MessagePreview>>(messageId.UserId);
        if (userMessages == null)
        {
            return;
        }

        userMessages.Remove(messageId);
        if (!userMessages.Any())
        {
            _previewsCache.Remove(messageId.UserId);
        }
    }
}

public class ContentsCache
{
    private readonly TimeSpan _cacheDuration;
    private readonly MemoryCache _contentsCache;

    public ContentsCache(int sizeLimit, TimeSpan cacheDuration)
    {
        _contentsCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = sizeLimit });
        _cacheDuration = cacheDuration;
    }

    public void AddOrUpdate(MessageId messageId, MessageContent content)
    {
        _contentsCache.Set(messageId, content, DateTimeOffset.UtcNow.Add(_cacheDuration));
    }

    public MessageContent? Get(MessageId messageId)
    {
        return _contentsCache.Get<MessageContent>(messageId);
    }

    public void Delete(MessageId messageId)
    {
        _contentsCache.Remove(messageId);
    }
}