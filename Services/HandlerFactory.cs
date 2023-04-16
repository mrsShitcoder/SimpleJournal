namespace Journal.Services;

public class JsonRpcHandlerFactory
{
    private readonly Dictionary<string, IJsonRpcHandlerBase> _commands = new();

    public JsonRpcHandlerFactory(JournalService journalService)
    {
        AddHandler(new GetPreviewsJsonRpc(journalService));
        AddHandler(new GetContentJsonRpc(journalService));
        AddHandler(new AddMessageJsonRpc(journalService));
        AddHandler(new DeleteMessageJsonRpc(journalService));
        AddHandler(new SetMessageSeenJsonRpc(journalService));
    }

    public IJsonRpcHandlerBase GetHandler(string method)
    {
        if(_commands.TryGetValue(method, out var command))
        {
            return command;
        }

        throw new ArgumentException($"Method {method} is not supported");
    }

    private void AddHandler(IJsonRpcHandlerBase command)
    {
        _commands[command.GetMethod()] = command;
    }
    
}
