using ProtoBuf;

namespace StriBot.Application.FileManager.Models
{
    [ProtoContract]
    public class UserCredentials
    {
        [ProtoMember(1)]
        public string Channel { get; set; }

        [ProtoMember(2)]
        public string ChannelId { get; set; }

        [ProtoMember(3)]
        public string ChannelAccessToken { get; set; }

        [ProtoMember(4)]
        public string BotName { get; set; }

        [ProtoMember(5)]
        public string BotAccessToken { get; set; }
    }
}