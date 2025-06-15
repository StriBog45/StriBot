using System.Threading.Tasks;
using StriBot.Application.FileManager;
using StriBot.Application.Twitch;
using StriBot.Application.Twitch.Interfaces;

namespace StriBot.Application.Loader;

public class Loader(ITwitchInfo twitchInfo, SettingsFileManager settingsFileManager, TwitchAuthorization twitchAuthorization)
{
    public async Task Load()
    {
        twitchInfo.Set(settingsFileManager.GetUserCredentials());
        await twitchAuthorization.RefreshTokens();
    }
}