using DryIoc;
using StriBot.Commands.CommonFunctions;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    class CommandCommonFunctions : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<ReadyMadePhrases>(Reuse.Singleton);
        }
    }
}