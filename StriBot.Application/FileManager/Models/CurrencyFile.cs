using ProtoBuf;

namespace StriBot.Application.FileManager.Models
{
    [ProtoContract]
    public class CurrencyFile
    {
        [ProtoMember(1)]
        public string CurrencyName { get; set; }
    }
}