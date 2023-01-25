using System;
using System.IO;
using System.Windows.Forms;
using StriBot.Application.Bot.Interfaces;

namespace StriBot
{
    internal class TwitchInfo : ITwitchInfo
    {
        public string Channel { get; }
        public string BotAccessToken { get; }
        public string ChannelAccessToken { get; }
        public string BotName { get; }
        public string BotClientId { get; }
        public string ChannelId { get; }

        private readonly string fileName = "TwitchInfo.txt";
        private readonly string error = "Error";

        public TwitchInfo()
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var line = streamReader.ReadToEnd().Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

                Channel = line[0].Split(new[] {':', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries)[1];
                BotAccessToken = line[1].Split(new[] {':', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries)[1];
                ChannelAccessToken = line[2].Split(new[] {':', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries)[1];
                BotName = line[3].Split(new[] {':', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries)[1];
                BotClientId = line[4].Split(new[] {':', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries)[1];
                ChannelId = line[5].Split(new[] {':', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries)[1];
            }
        }
    }
}