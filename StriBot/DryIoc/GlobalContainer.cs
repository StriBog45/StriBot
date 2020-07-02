using DryIoc;
using StriBot.DryIoc.Registrators;

namespace StriBot.DryIoc
{
    public static class GlobalContainer
    {
        public static IContainer Default { get; private set; }

        public static IContainer Initialize()
        {
            Default = InitializeContainer();

            return Default;
        }

        private static IContainer InitializeContainer()
            => new Container()
                .RegistrateFillers();
    }
}