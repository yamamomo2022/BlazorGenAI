using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.VisualBasic;

namespace BlazorGenAI
{
    public class AISpeech
    {
        const string SPEECH_KEY = "YOUR_SPEECH_KEY";
        const string SPEECH_REGION = "YOUR_SPEECH_REGION";

        // private SpeechRecognizer recognizer;

        public AISpeech()
        {
            var speechConfig = SpeechConfig.FromSubscription(SPEECH_KEY, SPEECH_REGION);
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
            

        }
    }
}



