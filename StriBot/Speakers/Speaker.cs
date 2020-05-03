using System.Speech.Synthesis;

namespace StriBot.Speakers
{
    public class Speaker
    {
        SpeechSynthesizer speechSynthesizer;

        public Speaker()
        {
            speechSynthesizer = new SpeechSynthesizer();

            // Configure the audio output.   
            speechSynthesizer.SetOutputToDefaultAudioDevice();
        }

        public void Say(string text)
        {
            speechSynthesizer.Speak(text);
        }
    }
}