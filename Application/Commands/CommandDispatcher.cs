namespace Application.Commands;

public class CommandDispatcher
{
    private readonly Dictionary<string, ICommand> _commands;

    public CommandDispatcher(IEnumerable<ICommand> commands) => _commands = commands.ToDictionary(c => c.Type, c => c);

    public ICommand? GetCommand(string type) => _commands.GetValueOrDefault(type);
}