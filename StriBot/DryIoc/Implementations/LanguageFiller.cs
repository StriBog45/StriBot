using DryIoc;
using StriBot.Application.Localization.Implementations;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    [FillPriority(2)]
    class LanguageFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<Currency>(Reuse.Singleton);
            container.Register<Minute>(Reuse.Singleton);
        }
    }
}