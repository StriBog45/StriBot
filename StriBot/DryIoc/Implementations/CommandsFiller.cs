using DryIoc;
using StriBot.Commands;
using StriBot.CustomData;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    class CommandsFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<MMRManager>(Reuse.Singleton);
            container.Register<OrderManager>(Reuse.Singleton);
            container.Register<CurrencyBaseManager>(Reuse.Singleton);
            container.Register<HalberdManager>(Reuse.Singleton);
            container.Register<DuelManager>(Reuse.Singleton);
            container.Register<LinkManager>();
            container.Register<RandomAnswerManager>(Reuse.Singleton);
            container.Register<BurgerManager>(Reuse.Singleton);
            container.Register<BetsManager>(Reuse.Singleton);
            container.Register<ProgressManager>(Reuse.Singleton);
            container.Register<RememberManager>(Reuse.Singleton);
            container.Register<RaffleManager>(Reuse.Singleton);
            container.Register<AnswerOptions>(Reuse.Singleton);
        }
    }
}