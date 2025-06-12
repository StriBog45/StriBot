using DryIoc;
using StriBot.Application.Speaker.Interfaces;
using StriBot.DryIoc.Interfaces;
using StriBot.Speakers;

namespace StriBot.DryIoc.Implementations;

class SpeakerFiller : IContainerFiller
{
    public void Fill(IContainer container)
    {
        container.Register<ISpeaker, Speaker>(Reuse.Singleton);
    }
}