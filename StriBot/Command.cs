using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace StriBot
{
    public class Command
    {
        public string Name { get; set; }
        public string Info { get; set; }
        public string[] Args { get; set; } = null;
        public Role Requires { get; set; }
        public CommandType Type { get; set; }
        public Action<OnChatCommandReceivedArgs> Action { get; set; }

        public Command(string name, string info, Role requires, Action<OnChatCommandReceivedArgs> action, CommandType type) : this(name, info, requires, action, null, type) { }

        public Command(string name, string info, Role requires, Action<OnChatCommandReceivedArgs> action, string[] args, CommandType type)
        {
            Requires = requires;
            Name = name;
            Info = info;
            Action = action;
            Args = args;
            Type = type;
        }

        public Command(string name, string info, Action<OnChatCommandReceivedArgs> action, string[] args, CommandType type) : this (name, info, Role.Any, action, args, type ) { }

        public Command(string name, string info, Action<OnChatCommandReceivedArgs> action, CommandType type) : this(name, info, Role.Any, action, null, type) { }
    }
}
