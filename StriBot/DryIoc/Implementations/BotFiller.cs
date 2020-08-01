using DryIoc;
using StriBot.Bots;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    [FillPriority(1)]
    class BotFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<ChatBot>(Reuse.Singleton);
            container.Register<TwitchBot>(Reuse.Singleton);
        }
    }
}