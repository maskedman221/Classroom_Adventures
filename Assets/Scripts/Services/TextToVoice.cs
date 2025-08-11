using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TextToVoice : MonoBehaviour
{

    [SerializeField] private string apiKey = "YOUR_API_KEY";
    [SerializeField] private string voiceId = "YOUR_VOICE_ID";
    public AudioSource audioSource;
    public event Action OnAudioClipEnd;
    public event Action OnAudioClipsEnd;

    [Serializable]
    public class VoiceSettings { public float stability = 0.5f; public float similarity_boost = 0.75f; }
    [Serializable]
    public class TTSRequest { public string text; public string model_id; public VoiceSettings voice_settings; }

    private List<AudioClip> audioClips = new List<AudioClip>();

    public void SpeakAll(List<string> sentences)
    {
        StartCoroutine(ProcessAllSentences(sentences));
    }

    private IEnumerator ProcessAllSentences(List<string> sentences)
    {
        audioClips.Clear();

        foreach (var sentence in sentences)
        {
            yield return StartCoroutine(GenerateClipFromText(sentence));
        }

        StartCoroutine(PlayAllAudioClips());
    }

    private IEnumerator GenerateClipFromText(string text)
    {
        string url = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}";

        var requestData = new TTSRequest
        {
            text = text,
            model_id = "eleven_monolingual_v1",
            voice_settings = new VoiceSettings()
        };

        string jsonBody = JsonUtility.ToJson(requestData);

        UnityWebRequest request = UnityWebRequest.Put(url, jsonBody);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("xi-api-key", apiKey);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.responseCode == 429)
        {
            Debug.LogWarning("Rate limited. Waiting 5 seconds...");
            yield return new WaitForSeconds(5);
            yield return StartCoroutine(GenerateClipFromText(text)); // retry
            yield break;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("TTS error: " + request.error);
            yield break;
        }

        string path = Path.Combine(Application.persistentDataPath, Guid.NewGuid() + ".mp3");
        File.WriteAllBytes(path, request.downloadHandler.data);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Audio load error: " + www.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioClips.Add(clip);
        }
    }

    private IEnumerator PlayAllAudioClips()
    {
        foreach (AudioClip clip in audioClips)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);
            OnAudioClipEnd?.Invoke();
        }
        OnAudioClipsEnd?.Invoke();
        Debug.Log("All audio clips played.");
    }
}
