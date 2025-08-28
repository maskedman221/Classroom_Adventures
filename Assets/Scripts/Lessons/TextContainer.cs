using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
public class TextContainer : MonoBehaviour
{
    [SerializeField] private TextToVoice ttv;
    [SerializeField] private GameObject imageTextTemplate;
    [SerializeField] private Transform textContainer;
    private List<string> sentences = new List<string>();
    private int index = 0;
    private void Awake()
    {
        imageTextTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
    ApiLessonsLoader.Instance.OnSentenceSet += HandleLessonUI;
    }

    private void HandleLessonUI(object sender, EventArgs e)
{
    sentences = ApiLessonsLoader.Instance.getSentences();
    index = 0; // reset index

    foreach (Transform child in textContainer)
    {
        if (child != imageTextTemplate.transform)
            Destroy(child.gameObject);
    }

    if (sentences.Count > 0)
    {
        TMP_Text tmp = imageTextTemplate.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = sentences[index];
        imageTextTemplate.SetActive(true);

        ttv.OnAudioClipEnd += OnAudioClipEnd_PresentTheNextText;
    }
    else
    {
        imageTextTemplate.SetActive(false);
    }
}

    private void OnAudioClipEnd_PresentTheNextText()
    {
        index++;
        if (index >= sentences.Count) return;
        GameObject nextImageText = Instantiate(imageTextTemplate, textContainer);
        TMP_Text tmp = nextImageText.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = sentences[index];
        nextImageText.gameObject.SetActive(true);
    }
}
