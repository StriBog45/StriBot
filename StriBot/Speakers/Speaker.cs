using System.Linq;
using System.Runtime.Versioning;
using System.Speech.Synthesis;
using StriBot.Application.Speaker.Interfaces;

namespace StriBot.Speakers;

[SupportedOSPlatform("windows")]
public class Speaker : ISpeaker
{
    private readonly SpeechSynthesizer _speechSynthesizer;

    public Speaker()
    {
        _speechSynthesizer = new SpeechSynthesizer();

        // Configure the audio output.   
        _speechSynthesizer.SetOutputToDefaultAudioDevice();

        var voices = _speechSynthesizer.GetInstalledVoices();

        // Голоса можно скачать тут https://rhvoice.su/voices/ (SAPI5)
        if (voices.Any(voice => voice.VoiceInfo.Name == "Tatiana"))
            _speechSynthesizer.SelectVoice("Tatiana");
    }

    public void Say(string text)
    {
        _speechSynthesizer.Speak(text);
    }
}