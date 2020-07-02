using DryIoc;
using StriBot.Commands;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    class CommandsFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<ManagerMMR>(Reuse.Singleton);
        }
    }
}