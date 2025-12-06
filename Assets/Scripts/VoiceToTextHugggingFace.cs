using UnityEngine;
using System.IO;
using UnityEngine.UI;
//using HuggingFace.API;
using UnityEngine.Windows.Speech;

public class VoiceToTextHugggingFace : MonoBehaviour
{
    [SerializeField] Button recordButton;
    [SerializeField] Button stopButton;

    AudioClip clip;
    byte[] clipData;
    bool recording;
    string result = "";
    bool errored = false;

    public string Result { get => result; }
    public bool Errored { get => errored; }

    public void StartRecording()
    {
        clip = Microphone.Start(null, false, 20, 44100);
        recording = true;

        recordButton.interactable = false;
        stopButton.interactable = true;
    }

    void Update()
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples) { StopRecording(); }
    }

    public void StopRecording()
    {
        int position = Microphone.GetPosition(null);
        Microphone.End(null);
        float[] samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        clipData = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();

        recordButton.interactable = true;
        stopButton.interactable = false;
    }

    void SendRecording()
    {
        /* Hugging Face an't working
        HuggingFaceAPI.AutomaticSpeechRecognition(clipData, response =>
        { result = response; errored = false; }, error => { result = error; errored = true; });
        if (errored)
        {
            Debug.LogError($"Speech Recognition Failed Error: {result}");
        }
        else
        {
            Debug.Log($"Speech Recognized: {result}");
        }
        */
    }

    byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (MemoryStream memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (float sample in samples)
                { writer.Write((short)(sample * short.MaxValue)); }
            }
            return memoryStream.ToArray();
        }
    }
}
