using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Windows.Speech;

public class VoiceToTextUnityBuiltin : MonoBehaviour
{
    [SerializeField] Button recordButton;
    [SerializeField] Button stopButton;
    [SerializeField] TMP_Text recognizedText;
    [SerializeField] Robot robot;

    string result = "";

    DictationRecognizer recognizer;

    void Awake()
    {
        recognizer = new DictationRecognizer(ConfidenceLevel.Low);

        recognizer.DictationComplete += (cause) => StopRecording();
        recognizer.DictationError += (error, hresult) => Debug.Log($"Failed to recognize speech error: {error}");
        //recognizer.DictationHypothesis += (text) => recognizedText.text = text;
        recognizer.DictationResult += (text, confidence) =>
        {
            recognizedText.text = text;
            result = text;
            Debug.Log($"Recognized Text: {text} Confidence: {confidence}");
            StopRecording();
            robot.TakeCommand(result);
        };
    }

    public void StartRecording()
    {
        recognizer.Start();
        recordButton.interactable = false;
        stopButton.interactable = true;
    }

    public void StopRecording()
    {
        recognizer.Stop();
        recordButton.interactable = true;
        stopButton.interactable = false;
    }
}
