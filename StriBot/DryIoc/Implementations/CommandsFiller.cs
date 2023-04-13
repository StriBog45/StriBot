using DryIoc;
using StriBot.Application.Commands.Handlers;
using StriBot.Application.Commands.Handlers.Progress;
using StriBot.Application.Commands.Handlers.Raffle;
using StriBot.Application.Localization;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    class CommandsFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<MMRHandler>(Reuse.Singleton);
            container.Register<OrderHandler>(Reuse.Singleton);
            container.Register<CurrencyBaseHandler>(Reuse.Singleton);
            container.Register<HalberdHandler>(Reuse.Singleton);
            container.Register<DuelHandler>(Reuse.Singleton);
            container.Register<CustomCommandHandler>();
            container.Register<RandomAnswerHandler>(Reuse.Singleton);
            container.Register<BurgerHandler>(Reuse.Singleton);
            container.Register<BetsHandler>(Reuse.Singleton);
            container.Register<ProgressHandler>(Reuse.Singleton);
            container.Register<RememberHandler>(Reuse.Singleton);
            container.Register<RaffleHandler>(Reuse.Singleton);
            container.Register<AnswerOptions>(Reuse.Singleton);
            container.Register<BananaHandler>();
        }
    }
}