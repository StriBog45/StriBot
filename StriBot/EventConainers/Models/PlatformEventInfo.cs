using StriBot.Bots.Enums;
using StriBot.EventConainers.Enums;

namespace StriBot.EventConainers.Models
{
    public class PlatformEventInfo
    {
        public PlatformEventInfo(PlatformEventType eventType, Platform platform, string displayName = null, string message = null)
        {
            EventType = eventType;
            Platform = platform;
            UserName = displayName;
            Message = message;
        }

        public PlatformEventType EventType { get; }

        public Platform Platform { get; }

        public string UserName { get; }

        public string Message { get; }
    }
}