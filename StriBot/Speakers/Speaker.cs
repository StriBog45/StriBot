using System.Speech.Synthesis;

namespace StriBot.Speakers
{
    public class Speaker
    {
        private readonly SpeechSynthesizer _speechSynthesizer;

        public Speaker()
        {
            _speechSynthesizer = new SpeechSynthesizer();

            // Configure the audio output.   
            _speechSynthesizer.SetOutputToDefaultAudioDevice();
        }

        public void Say(string text)
        {
            _speechSynthesizer.Speak(text);
        }
    }
}