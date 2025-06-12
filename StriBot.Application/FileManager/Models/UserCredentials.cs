using ProtoBuf;

namespace StriBot.Application.FileManager.Models;

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
    public string ChannelRefreshToken { get; set; }

    [ProtoMember(5)] 
    public int ChannelExpiresIn { get; set; }

    [ProtoMember(6)]
    public string BotName { get; set; }

    [ProtoMember(7)]
    public string BotAccessToken { get; set; }

    [ProtoMember(8)]
    public string BotRefreshToken { get; set; }

    [ProtoMember(9)]
    public int BotExpiresIn { get; set; }
}