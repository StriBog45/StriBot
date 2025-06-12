using ProtoBuf;

namespace StriBot.Application.FileManager.Models;

[ProtoContract]
public class UserSettings
{
    [ProtoMember(1)]
    public string CurrencyName { get; set; }

    [ProtoMember(2)]
    public UserCredentials UserCredentials { get; set; }
}