using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LessonManager : MonoBehaviour
{
    [SerializeField] private TextToVoice ttv;
    [SerializeField] private GameObject nextButton;
    private int currentSentence = 0;
    private List<string> sentences = new List<string>();

    private void Awake()
    {
        nextButton.gameObject.SetActive(false);
    }
    private void Start()
    {
        sentences = ApiLessonsLoader.Instance.getSentences();
        ttv.OnAudioClipsEnd += OnAudioClipsEnd_RevelTheNextButton;
        Debug.Log(string.Join(", ", sentences));
        if (ttv != null && sentences.Count > 0)
        {
            Debug.Log("start reading");
            ttv.SpeakAll(sentences);
        }
    }

    private void OnAudioClipsEnd_RevelTheNextButton()
    {
        nextButton.gameObject.SetActive(true);
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
