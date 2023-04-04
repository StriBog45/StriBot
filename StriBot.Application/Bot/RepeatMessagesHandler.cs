using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using StriBot.Application.Configurations.Models;
using StriBot.Application.Events;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Bot
{
    public class RepeatMessagesHandler
    {
        private List<RepeatMessage> _repeatMessages;

        public RepeatMessagesHandler(IConfiguration configuration)
        {
            _repeatMessages = configuration.GetSection("RepeatMessages").Get<List<RepeatMessage>>();
        }

        public void Tick(int timer)
        {
            if (_repeatMessages != null)
            {
                foreach (var repeatMessage in _repeatMessages)
                {
                    if (timer >= repeatMessage.DelayBeforeFirstDispatchInMinutes
                        && (timer - repeatMessage.DelayBeforeFirstDispatchInMinutes) % repeatMessage.FrequencyInMinutes == 0)
                    {
                        EventContainer.Message(repeatMessage.Message, new[] {Platform.Twitch});
                    }
                }
            }
        }
    }
}