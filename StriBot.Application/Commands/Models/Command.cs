using System;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Events.Models;

namespace StriBot.Application.Commands.Models;

public class Command
{
    public string Name { get; set; }
    public string Info { get; set; }
    public string[] Args { get; set; } = null;
    public Role Requires { get; set; }
    public CommandType Type { get; set; }
    public Action<CommandInfo> Action { get; set; }

    public Command(string name, string info, Role requires, Action<CommandInfo> action, CommandType type) : this(name, info, requires, action, null, type) { }

    public Command(string name, string info, Role requires, Action<CommandInfo> action, string[] args, CommandType type)
    {
        Requires = requires;
        Name = name;
        Info = info;
        Action = action;
        Args = args;
        Type = type;
    }

    public Command(string name, string info, Action<CommandInfo> action, string[] args, CommandType type) : this(name, info, Role.Any, action, args, type) { }

    public Command(string name, string info, Action<CommandInfo> action, CommandType type) : this(name, info, Role.Any, action, null, type) { }
}