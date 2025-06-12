using StriBot.Application.Events.Enums;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Events.Models;

public class PlatformEventInfo
{
    public PlatformEventInfo(PlatformEventType eventType, Platform platform, string displayName = null, string message = null, string secondName = null)
    {
        EventType = eventType;
        Platform = platform;
        UserName = displayName;
        Message = message;
        SecondName = secondName;
    }

    public PlatformEventType EventType { get; }

    public Platform Platform { get; }

    public string UserName { get; }

    public string Message { get; }

    /// <summary>
    /// Имя второго пользователя
    /// </summary>
    public string SecondName { get; }
}