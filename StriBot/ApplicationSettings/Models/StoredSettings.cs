using ProtoBuf;

namespace StriBot.ApplicationSettings.Models
{
    [ProtoContract]
    public class StoredSettings
    {
        [ProtoMember(1)]
        public string CurrencyName { get; set; }
    }
}