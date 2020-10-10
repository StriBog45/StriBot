using DryIoc;
using StriBot.DryIoc.Interfaces;
using StriBot.Language.Implementations;

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