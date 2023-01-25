using System.IO;
using ProtoBuf;
using StriBot.Application.FileManager.Models;

namespace StriBot.Application.FileManager
{
    public class SettingsFileManager
    {
        private const string FileName = "UserSettings";
        private readonly string _fileNameWithExtension = $"{FileName}.bin";
        private CurrencyFile _currencyFile;

        public SettingsFileManager()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (File.Exists(_fileNameWithExtension))
            {
                using (var file = File.OpenRead(_fileNameWithExtension))
                {
                    _currencyFile = Serializer.Deserialize<CurrencyFile>(file);
                }
            }
            else
            {
                _currencyFile = new CurrencyFile();
            }
        }

        public void SaveSettings()
        {
            using (var file = File.Create(_fileNameWithExtension))
            {
                Serializer.Serialize(file, _currencyFile);
            }
        }

        public string CurrencyName
            => _currencyFile.CurrencyName;

        public void SetCurrencyName(string name)
            => _currencyFile.CurrencyName = name;
    }
}