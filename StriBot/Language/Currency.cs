namespace StriBot.Language
{
    public class Currency : ICases
    {
        /// <summary>
        ///  Кто? Что?
        /// </summary>
        public string Nominative => "кристалл";//"игрушка";

        /// <summary>
        /// Кого? Чего?
        /// </summary>
        public string Genitive => "кристалла";//"игрушки";

        /// <summary>
        /// Кому? Чему?
        /// </summary>
        public string Dative => "кристаллу";//"игрушку";

        /// <summary>
        /// Кого? Что?
        /// </summary>
        public string Accusative => "кристалл";//"игрушку";

        /// <summary>
        /// Кем? Чем?
        /// </summary>
        public string Instrumental => "кристаллом";//"игрушкой";

        /// <summary>
        /// О ком? О чем?
        /// </summary>
        public string Prepositional => "кристалле";//"игрушке";

        public string NominativeMultiple => "кристаллы";//"игрушки";

        public string GenitiveMultiple => "кристаллов";//"игрушек";

        public string DativeMultiple => "кристаллам";//"игрушкам";

        public string AccusativeMultiple => "кристаллы";//"игрушки";

        public string InstrumentalMultiple => "кристаллами";//"игрушками";

        public string PrepositionalMultiple => "кристаллах";//"игрушках";
    }
}