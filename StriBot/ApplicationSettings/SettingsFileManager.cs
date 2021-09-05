using ProtoBuf;
using System.IO;
using StriBot.ApplicationSettings.Models;

namespace StriBot.ApplicationSettings
{
    public class SettingsFileManager
    {
        private const string FileName = "UserSettings";
        private readonly string _fileNameWithExtension = $"{FileName}.bin";
        private StoredSettings _storedSettings;

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
                    _storedSettings = Serializer.Deserialize<StoredSettings>(file);
                }
            }
            else
            {
                _storedSettings = new StoredSettings();
            }
        }

        public void SaveSettings()
        {
            using (var file = File.Create(_fileNameWithExtension))
            {
                Serializer.Serialize(file, _storedSettings);
            }
        }

        public string CurrencyName
            => _storedSettings.CurrencyName;

        public void SetCurrencyName(string name)
            => _storedSettings.CurrencyName = name;
    }
}