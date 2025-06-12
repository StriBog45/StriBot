using DryIoc;
using StriBot.Application.DataBase.Interfaces;
using StriBot.DateBase.Implementations;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations;

public class DataBaseFiller : IContainerFiller
{
    public void Fill(IContainer container)
    {
        container.Register<IDataBase, DataBase>(Reuse.Singleton);
    }
}