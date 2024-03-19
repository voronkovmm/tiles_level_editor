using System;
using System.Collections.Generic;

public class CommandExecutor<T> where T : ICommandModel
{
    private Dictionary<Type, object> commands = new();

    public void AddCommand<TCommand>(AbstractLevelEditorCommand<TCommand> command) where TCommand : struct, T
    {
        Type commandType = typeof(TCommand);
        if (!commands.ContainsKey(commandType))
            commands[commandType] = command;
    }

    public void Execute<TCommand>(in TCommand arg) where TCommand : struct, T
    {
        Type commandType = typeof(TCommand);
        if (commands.ContainsKey(commandType))
            (commands[commandType] as AbstractLevelEditorCommand<TCommand>)?.Execute(arg);
    }
}