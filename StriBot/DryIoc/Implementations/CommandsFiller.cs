﻿using DryIoc;
using StriBot.Commands;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Implementations
{
    class CommandsFiller : IContainerFiller
    {
        public void Fill(IContainer container)
        {
            container.Register<MMRManager>(Reuse.Singleton);
            container.Register<OrderManager>(Reuse.Singleton);
        }
    }
}