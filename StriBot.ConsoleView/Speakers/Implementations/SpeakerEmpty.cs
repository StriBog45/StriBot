using StriBot.Application.Speaker.Interfaces;

namespace StriBot.ConsoleView.Speakers.Implementations;

public class SpeakerEmpty : ISpeaker
{
    public void Say(string text)
    {
    }
}