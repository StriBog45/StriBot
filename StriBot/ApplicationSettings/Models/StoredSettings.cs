using ProtoBuf;

namespace StriBot
{
    [ProtoContract]
    public class StoredSettings
    {
        [ProtoMember(1)]
        public string CurrencyName { get; set; }
    }
}