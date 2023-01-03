using DryIoc;
using StriBot.DateBase.Implementations;
using StriBot.DateBase.Interfaces;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    public class DataBaseFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<IDataBase, DataBase>(Reuse.Singleton);
        }
    }
}