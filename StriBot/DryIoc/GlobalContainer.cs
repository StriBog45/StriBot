using DryIoc;
using StriBot.DryIoc.Registrators;

namespace StriBot.DryIoc
{
    public static class GlobalContainer
    {
        public static IContainer Default { get; private set; }

        public static void Initialize()
        {
            Default = InitializeContainer();
        }

        private static IContainer InitializeContainer()
            => new Container()
                .RegistrateFillers();
    }
}