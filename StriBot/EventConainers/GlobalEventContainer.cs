using StriBot.Bots.Enums;
using StriBot.EventConainers.Models;
using System;
using System.Windows.Forms;

namespace StriBot.EventConainers
{
    public static class GlobalEventContainer
    {
        public delegate void CommandHandler(CommandInfo commandInfo);
        public delegate void MessageHandler(Platform[] platforms, string message);
        public delegate void PlatformEventHandler(PlatformEventInfo platformEventInfo);
        public static event CommandHandler CommandReceived;
        public static event MessageHandler SendMessage;
        public static event PlatformEventHandler PlatformEventReceived;

        public static void CreateEventCommandCall(CommandInfo commandInfo)
        {
            try
            {
                CommandReceived?.Invoke(commandInfo);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.StackTrace}{Environment.NewLine}{exception.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Message(string message, Platform[] platforms)
        {
            try
            {
                SendMessage?.Invoke(platforms, message);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.StackTrace}{Environment.NewLine}{exception.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Message(string message, Platform platform)
            => Message(message, new[] { platform });

        public static void Event(PlatformEventInfo platformEventInfo)
            => PlatformEventReceived?.Invoke(platformEventInfo);
    }
}