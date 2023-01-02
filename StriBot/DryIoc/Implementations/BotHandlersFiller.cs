using DryIoc;
using StriBot.Bots.Handlers;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    public class BotHandlersFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<RewardHandler>(Reuse.Singleton);
        }
    }
}