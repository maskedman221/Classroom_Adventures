using UnityEngine;
using System.Collections.Generic;
using System;

public class ApiLessonsLoader : MonoBehaviour
{
   public static ApiLessonsLoader Instance { get; private set; }
   private List<string> sentences = new List<string>();
   private string sentence ;
   public event EventHandler OnSentenceSet;
   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   { 

   }

public List<string> SetSentences(string description)
{
    sentences.Clear(); // ✅ reset list

    int startIndex = 0;

    for (int i = 0; i < description.Length; i++)
    {
        if (description[i] == '.')
        {
            int length = i - startIndex + 1;
            string newSentence = description.Substring(startIndex, length).Trim();
            sentences.Add(newSentence);
            startIndex = i + 1; // ✅ start after the dot
        }
    }

    // Add any remaining text as a sentence
    if (startIndex < description.Length)
    {
        string lastSentence = description.Substring(startIndex).Trim();
        if (!string.IsNullOrEmpty(lastSentence))
            sentences.Add(lastSentence);
    }

    OnSentenceSet?.Invoke(this, EventArgs.Empty);

    return sentences;
}

   public List<string> getSentences()
   {
      return sentences;
   }
}
