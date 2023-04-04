using DryIoc;
using StriBot.Application.Bot;
using StriBot.Application.Bot.Interfaces;
using StriBot.Application.FileManager;
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
            container.Register<TwitchApiClient>(Reuse.Singleton);
            container.Register<RepeatMessagesHandler>(Reuse.Singleton);
        }
    }
}