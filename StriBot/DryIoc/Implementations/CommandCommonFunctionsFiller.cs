using DryIoc;
using StriBot.Application.Localization;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations;

class CommandCommonFunctionsFiller : IContainerFiller
{
    public void Fill(IContainer container)
    {
        container.Register<ReadyMadePhrases>(Reuse.Singleton);
    }
}