using ProtoBuf;

namespace StriBot.Language.Models
{
    [ProtoContract]
    public class Cases
    {
        /// <summary>
        ///  Кто? Что?
        /// </summary>
        [ProtoMember(1)]
        public string Nominative { get; set; }

        /// <summary>
        /// Кого? Чего?
        /// </summary>
        [ProtoMember(2)]
        public string Genitive { get; set; }

        /// <summary>
        /// Кому? Чему?
        /// </summary>
        [ProtoMember(3)]
        public string Dative { get; set; }

        /// <summary>
        /// Кого? Что?
        /// </summary>
        [ProtoMember(4)]
        public string Accusative { get; set; }

        /// <summary>
        /// Кем? Чем?
        /// </summary>
        [ProtoMember(5)]
        public string Instrumental { get; set; }

        /// <summary>
        /// О ком? О чем?
        /// </summary>
        [ProtoMember(6)]
        public string Prepositional { get; set; }

        [ProtoMember(7)]
        public string NominativeMultiple { get; set; }

        [ProtoMember(8)]
        public string GenitiveMultiple { get; set; }

        [ProtoMember(9)]
        public string DativeMultiple { get; set; }

        [ProtoMember(10)]
        public string AccusativeMultiple { get; set; }

        [ProtoMember(11)]
        public string InstrumentalMultiple { get; set; }

        [ProtoMember(12)]
        public string PrepositionalMultiple { get; set; }

        [ProtoMember(13)]
        public string TestValue { get; set; }
    }
}