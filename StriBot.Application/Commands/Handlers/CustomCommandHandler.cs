using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Models;
using StriBot.Application.Configurations.Models;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;

namespace StriBot.Application.Commands.Handlers
{
    public class CustomCommandHandler
    {
        private readonly IConfiguration _configuration;

        public CustomCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Dictionary<string, Command> CreateCommands()
        {
            var customCommands = _configuration.GetSection("CustomCommands").Get<List<CustomCommand>>();
            var friendlyStreamers = _configuration.GetSection("FriendlyStreamers").Get<List<CustomCommand>>();

            var dictionaryCommands = new Dictionary<string, Command>();

            foreach (var customCommand in customCommands)
            {
                dictionaryCommands.Add(customCommand.Command.ToLower(), new Command(customCommand.Command, customCommand.Description,
                    delegate(CommandInfo commandInfo) { EventContainer.Message(customCommand.Message, commandInfo.Platform); },
                    CommandType.Info));
            }

            foreach (var friendlyStreamer in friendlyStreamers)
            {
                dictionaryCommands.Add(friendlyStreamer.Command.ToLower(), new Command(friendlyStreamer.Command, friendlyStreamer.Description,
                    delegate(CommandInfo commandInfo) { EventContainer.Message(friendlyStreamer.Message, commandInfo.Platform); },
                    CommandType.Streamers));
            }

            return dictionaryCommands;
        }
    }
}