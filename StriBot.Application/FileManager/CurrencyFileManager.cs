using System.IO;
using ProtoBuf;
using StriBot.Application.FileManager.Models;

namespace StriBot.Application.FileManager;

public class SettingsFileManager
{
    private const string FileName = "UserSettings";
    private readonly string _fileNameWithExtension = $"{FileName}.bin";
    private UserSettings _userSettings;

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
                _userSettings = Serializer.Deserialize<UserSettings>(file);
            }
        }
        else
        {
            _userSettings = new UserSettings();
        }
    }

    public void SaveSettings(string selectedCurrency,UserCredentials userCredentials)
    {
        _userSettings.CurrencyName = selectedCurrency;
        _userSettings.UserCredentials = userCredentials;

        using (var file = File.Create(_fileNameWithExtension))
        {
            Serializer.Serialize(file, _userSettings);
        }
    }

    public string CurrencyName
        => _userSettings.CurrencyName;

    public UserCredentials GetUserCredentials()
        => _userSettings.UserCredentials;
}