using Journal.Models;
using Microsoft.Extensions.Options;

namespace Journal.Services;

using MongoDB.Driver;

public class JournalDatabaseService
{

    private readonly IMongoCollection<Message> _messages;

    private readonly IMongoCollection<MessageId> _messageIds;

    private readonly MongoClient _mongoClient;

    public JournalDatabaseService(IOptions<JournalDatabaseSettings> dbSettings)
    {
        _mongoClient = new MongoClient(dbSettings.Value.Connection);
        var database = _mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _messages = database.GetCollection<Message>(dbSettings.Value.MessagesCollection);
        _messageIds = database.GetCollection<MessageId>(dbSettings.Value.CurrentMessageIdCollection);
    }

    public async Task<List<Message>> GetMessageList(MessageId messageId, int count)
    {
        IAsyncCursor<Message> cursor =  await _messages.FindAsync(
            message => (message.Id.UserId == messageId.UserId) && (message.Id.Sequence > messageId.Sequence),
            new FindOptions<Message> { Limit = count });

        return await cursor.ToListAsync();
    }

    public async Task<Message?> GetMessage(MessageId messageId)
    {
        return await _messages.Find(message => message.Id == messageId).FirstOrDefaultAsync();
    }

    public async Task<MessageId> CreateNewMessageId(ulong userId)
    {
        MessageId currentId =
            await _messageIds.Find(sequence => sequence.UserId == userId).FirstOrDefaultAsync();
        
        if (currentId == null)
        {
            currentId = new MessageId();
            currentId.UserId = userId;
            await _messageIds.InsertOneAsync(currentId);
        }
        else
        {
            currentId.Sequence++;
            var updateId = Builders<MessageId>.Update.Set(messageId => messageId.Sequence, currentId.Sequence);
            await _messageIds.UpdateOneAsync(messageId => messageId.UserId == userId, updateId);
        }

        return currentId;
    }

    public async Task CreateMessage(Message message)
    {
        await _messages.InsertOneAsync(message);
    }

    public async Task DeleteMessage(MessageId messageId)
    {
        await _messages.DeleteOneAsync(message => message.Id == messageId);
    }

    public async Task UpdateMessageState(MessageId messageId, MessageState state)
    {
        var updateState = Builders<Message>.Update.Set(preview => preview.State, state);
        await _messages.UpdateOneAsync<Message>(preview => preview.Id == messageId,
            updateState);
    }
}