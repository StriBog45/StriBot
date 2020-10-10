using ProtoBuf;
using StriBot.Language.Interfaces;
using StriBot.Language.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib.Api.Core.Models.Undocumented.ChatUser;

namespace StriBot.Language.Implementations
{
    public class Currency : ICases
    {
        private const string _catalog = "Currencies";
        private string _currencyName;
        private Cases _cases;

        public Currency()
        {
            _cases = new Cases();

            GetCurrencies();
        }

        ///<inheritdoc>
        public string Nominative => _cases.Nominative;

        ///<inheritdoc>
        public string Genitive => _cases.Genitive;

        ///<inheritdoc>
        public string Dative => _cases.Dative;

        ///<inheritdoc>
        public string Accusative => _cases.Accusative;

        ///<inheritdoc>
        public string Instrumental => _cases.Instrumental;

        ///<inheritdoc>
        public string Prepositional => _cases.Prepositional;

        ///<inheritdoc>
        public string NominativeMultiple => _cases.NominativeMultiple;

        ///<inheritdoc>
        public string GenitiveMultiple => _cases.GenitiveMultiple;

        ///<inheritdoc>
        public string DativeMultiple => _cases.DativeMultiple;

        ///<inheritdoc>
        public string AccusativeMultiple => _cases.AccusativeMultiple;

        ///<inheritdoc>
        public string InstrumentalMultiple => _cases.InstrumentalMultiple;

        ///<inheritdoc>
        public string PrepositionalMultiple => _cases.PrepositionalMultiple;

        public string[] GetCurrencies()
        {
            var result = new string[0];

            if (Directory.Exists(_catalog))
            {
                result = Directory.GetFiles(_catalog)
                    .Select(path => Path.GetFileNameWithoutExtension(path))
                    .ToArray();
            }
            else
            {
                Directory.CreateDirectory(_catalog);
            }

            return result;
        }

        public void LoadCurrency(string currencyName)
        {
            using (var file = File.OpenRead($"{_catalog}//{currencyName}.bin"))
            {
                _cases = Serializer.Deserialize<Cases>(file);
            }
            _currencyName = currencyName;
        }

        public bool CreateCurrency(string currencyName, Cases cases)
        {
            var result = false;

            if (IsValidFileName(currencyName))
            {
                using (var file = File.Create($"{_catalog}//{currencyName}.bin"))
                {
                    Serializer.Serialize(file, cases);
                }

                _currencyName = currencyName;
                _cases = cases;

                result = true;
            }

            return result;
        }

        private bool IsValidFileName(string fileName)
        {
            var invalidFileNameChars = Path.GetInvalidFileNameChars();

            foreach (var invalidChar in invalidFileNameChars)
            {
                foreach (var symbol in fileName)
                {
                    if (symbol == invalidChar)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}