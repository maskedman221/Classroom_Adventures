using UnityEngine;
using System.Collections.Generic;

public class ApiLessonsLoader : MonoBehaviour
{
   public static ApiLessonsLoader Instance { get; private set; }
   private List<string> sentences = new List<string>();
   private string sentence = "Hello!. " + "Welcome to the game. " + "Let's start learning.";
   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      int startIndex = 0;
      int endIndex = 0;
      int length = 0;
      for (int i = 0; i < sentence.Length; i++)
      {
         if (sentence[i] == '.')
         {
            endIndex = i;
            length = endIndex - startIndex + 1;
            string newSentence = sentence.Substring(startIndex, length);
            startIndex = i;
            sentences.Add(newSentence);
         }
      }
      Debug.Log(sentences);
   }


   public List<string> getSentences()
   {
      return sentences;
   }
}
