using StriBot.Application.Localization.Interfaces;

namespace StriBot.Application.Localization.Implementations
{
    public class Minute : ICases
    {
        public string Nominative => "минута";

        public string Genitive => "минуты";

        public string Dative => "минуту";

        public string Accusative => "минуту";

        public string Instrumental => "минутой";

        public string Prepositional => "минуте";

        public string NominativeMultiple => "минуты";

        public string GenitiveMultiple => "минут";

        public string DativeMultiple => "минутам";

        public string AccusativeMultiple => "минуты";

        public string InstrumentalMultiple => "минутами";

        public string PrepositionalMultiple => "минутах";
    }
}