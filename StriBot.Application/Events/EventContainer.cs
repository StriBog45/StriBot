using System.Threading.Tasks;
using StriBot.Application.Events.Models;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Events;

public static class EventContainer
{
    public delegate void CommandHandler(CommandInfo commandInfo);
    public delegate void MessageHandler(Platform[] platforms, string message);
    public delegate void PlatformEventHandler(PlatformEventInfo platformEventInfo);
    public delegate Task RewardEventHandler(RewardInfo rewardInfo);
    public delegate void MessageToApplicationHandler(string message, string title);
    public static event CommandHandler CommandReceived;
    public static event MessageHandler SendMessage;
    public static event MessageToApplicationHandler MessageToApplicationEvent; 
    public static event PlatformEventHandler PlatformEventReceived;
    public static event RewardEventHandler RewardEventReceived;

    public static void CreateEventCommandCall(CommandInfo commandInfo)
        => CommandReceived?.Invoke(commandInfo);

    public static void Message(string message, Platform[] platforms)
        => SendMessage?.Invoke(platforms, message);

    public static void Message(string message, Platform platform)
        => Message(message, new[] { platform });

    public static void Event(PlatformEventInfo platformEventInfo)
        => PlatformEventReceived?.Invoke(platformEventInfo);

    public static void RewardEvent(RewardInfo rewardInfo)
        => RewardEventReceived?.Invoke(rewardInfo);

    public static void MessageToApplication(string message, string title)
        => MessageToApplicationEvent?.Invoke(message, title);
}