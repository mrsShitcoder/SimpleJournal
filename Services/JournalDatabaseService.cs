using Journal.Models;
using Microsoft.Extensions.Options;

namespace Journal.Services;

using MongoDB.Driver;

public class JournalDatabaseService
{
    private readonly IMongoCollection<MessageContent> _messageContents;

    private readonly IMongoCollection<MessagePreview> _messagePreviews;

    private readonly IMongoCollection<UserSequence> _userSequence;

    private readonly MongoClient _mongoClient;

    public JournalDatabaseService(IOptions<JournalDatabaseSettings> dbSettings)
    {
        _mongoClient = new MongoClient(dbSettings.Value.Connection);
        var database = _mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _messagePreviews = database.GetCollection<MessagePreview>(dbSettings.Value.MessagePreviewsCollection);
        _messageContents = database.GetCollection<MessageContent>(dbSettings.Value.MessageContentsCollection);
        _userSequence = database.GetCollection<UserSequence>(dbSettings.Value.UserSequenceCollection);
    }

    public async Task<List<MessagePreview>> GetMessagePreviewsAsync(MessageId messageId, int count)
    {
        IAsyncCursor<MessagePreview> cursor =  await _messagePreviews.FindAsync(
            preview => (preview.Id.UserId == messageId.UserId) && (preview.Id.Sequence > messageId.Sequence),
            new FindOptions<MessagePreview> { Limit = count });

        return await cursor.ToListAsync();
    }

    public async Task<UserSequence?> GetUserSequenceAsync(ulong userId)
    {
        return await _userSequence.Find(sequence => sequence.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task AddUserSequenceAsync(UserSequence newSeq)
    {
        await _userSequence.ReplaceOneAsync(sequence => sequence.UserId == newSeq.UserId, newSeq);
    }

    public async Task<MessageContent?> GetMessageContentAsync(MessageId messageId)
        => await _messageContents.Find(message => message.Id == messageId).FirstOrDefaultAsync();

    public async Task CreateMessage(MessagePreview messagePreview, MessageContent messageContent)
    {
        using (var clientSession = _mongoClient.StartSession())
        {
            try
            {
                clientSession.StartTransaction();
                await _messagePreviews.InsertOneAsync(clientSession, messagePreview);
                await _messageContents.InsertOneAsync(clientSession, messageContent);
                await clientSession.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Transaction aborted. Error {e}");
                await clientSession.AbortTransactionAsync();
                throw;
            }
            
        }
    }

    public async Task DeleteMessage(MessageId messageId)
    {
        using (var clientSession = _mongoClient.StartSession())
        {
            try
            {
                clientSession.StartTransaction();
                await _messagePreviews.DeleteOneAsync(clientSession, message => message.Id == messageId);
                await _messageContents.DeleteOneAsync(clientSession, message => message.Id == messageId);
                await clientSession.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Transaction aborted. Error {e}");
                await clientSession.AbortTransactionAsync();
                throw;
            }
            
        }
    }
}