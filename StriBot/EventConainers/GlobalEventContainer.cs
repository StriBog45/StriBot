using StriBot.Bots.Enums;
using StriBot.EventConainers.Models;

namespace StriBot.EventConainers
{
    public static class GlobalEventContainer
    {
        public delegate void CommandHandler(CommandInfo commandInfo);
        public delegate void MessageHandler(Platform[] platforms, string message);
        public static event CommandHandler CommandReceived;
        public static event MessageHandler SendMessage;

        public static void CreateEventCommandCall(CommandInfo commandInfo)
            => CommandReceived?.Invoke(commandInfo);

        public static void Message(string message, Platform[] platforms)
            => SendMessage?.Invoke(platforms, message);

        public static void Message(string message, Platform platform)
            => Message(message, new Platform[] { platform });
    }
}