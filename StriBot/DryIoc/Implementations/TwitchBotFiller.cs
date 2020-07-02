using DryIoc;
using StriBot.DryIoc.Interfaces;
using StriBot.TwitchBot.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    [FillPriority(1)]
    class TwitchBotFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<ITwitchBot, TwitchBot.Implementations.TwitchBot>(Reuse.Singleton);
        }
    }
}