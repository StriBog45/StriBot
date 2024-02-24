using DryIoc;
using StriBot.Application.Bot;
using StriBot.Application.FileManager;
using StriBot.Application.Twitch;
using StriBot.Application.Twitch.Interfaces;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    [FillPriority(1)]
    class BotFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<ITwitchInfo, TwitchInfo>(Reuse.Singleton);
            container.Register<SettingsFileManager>(Reuse.Singleton);
            container.Register<ChatBot>(Reuse.Singleton);
            container.Register<TwitchBot>(Reuse.Singleton);
            container.Register<TwitchApiClient>();
            container.Register<TwitchAuthorization>();
            container.Register<RepeatMessagesHandler>(Reuse.Singleton);
        }
    }
}