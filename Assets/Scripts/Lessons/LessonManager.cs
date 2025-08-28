using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
public class LessonManager : MonoBehaviour
{
    [SerializeField] private TextToVoice ttv;
    [SerializeField] private Button nextButton;
    // private int currentSentence = 0;
    private int index = 0;
    private List<string> sentences = new List<string>();
    ApiGetLoader api = new ApiGetLoader();
    private void Awake()
    {
        nextButton.gameObject.SetActive(false);
    }
    private void Start()
    {
        nextButton.onClick.AddListener(() => NextLOG());
        StageManager.Instance.OnStageLoaded += HandleStageLoaded;
    }
  private void HandleStageLoaded(object sender, EventArgs e)
{
    index = 0; // ✅ start from first description
    ConvertToVoice();
}
    private void OnAudioClipsEnd_RevelTheNextButton()
    {
        nextButton.gameObject.SetActive(true);
    }
    private void NextLOG()
{
    index++; // move to next
    if (index >= LessonsData.descriptions.Count)
    {
        // reached the end
        if (MapData.gamemode == "PHOTO")
        {
            SceneManager.LoadScene(1);
        }
        if (MapData.gamemode == "MATH")
        {
            SceneManager.LoadScene(6);
        }
        if (MapData.gamemode == "QUIZ")
        {
            SceneManager.LoadScene(5);
        }
        return; // stop before trying to access out-of-range
    }

    ConvertToVoice();
}

    private void ConvertToVoice()
    {
        sentences = ApiLessonsLoader.Instance.SetSentences(LessonsData.descriptions[index]);
        ttv.OnAudioClipsEnd += OnAudioClipsEnd_RevelTheNextButton;
        Debug.Log(string.Join(", ", sentences));
        if (ttv != null && sentences.Count > 0)
        {
            Debug.Log("start reading");
            ttv.SpeakAll(sentences);
        }
    }
    // private void ReadNext()
    // {
    //     if (currentSentence < sentences.Count)
    //     {
    //         string sentence = sentences[currentSentence];
    //         ttv.Speak(sentence, () =>
    //         {
    //             ReadNext();
    //         });
    //     }
    //     else
    //     {
    //         Debug.Log("All Sentences Has been Readed!");
    //     }
    // }
}
