using DryIoc;
using StriBot.DryIoc.Interfaces;
using StriBot.Language;

namespace StriBot.DryIoc.Implementations
{
    class LanguageFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<Currency>(Reuse.Singleton);
        }
    }
}