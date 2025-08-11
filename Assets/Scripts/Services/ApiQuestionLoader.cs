using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class ApiQuestionLoader : MonoBehaviour
{
    public static ApiQuestionLoader Instance { get; private set; }
    private string apiUrl = "https://opentdb.com/api.php?amount=5&type=multiple";

    [System.Serializable]
    private class ApiQuestion
    {
        public string question;
        public string correct_answer;
        public List<string> incorrect_answers;
    }

    [System.Serializable]
    private class ApiResponse
    {
        public int response_code;
        public List<ApiQuestion> results;
    }

    public class ProcessedQuestion
    {
        public string question;
        public string[] allAnswers; // 4 choices (1 correct + 3 wrong)
        public int correctAnswerIndex; // Position of correct answer (0-3)
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
           
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Initialize()
    {
        var questions = await LoadRandomizedQuestions();
        if (questions != null)
        {
            Debug.Log($"Loaded {questions.Count} randomized questions");
            foreach (var q in questions)
            {
                Debug.Log($"\nQuestion: {q.question}");
                Debug.Log($"A) {q.allAnswers[0]} {(q.correctAnswerIndex == 0 ? "(CORRECT)" : "")}");
                Debug.Log($"B) {q.allAnswers[1]} {(q.correctAnswerIndex == 1 ? "(CORRECT)" : "")}");
                Debug.Log($"C) {q.allAnswers[2]} {(q.correctAnswerIndex == 2 ? "(CORRECT)" : "")}");
                Debug.Log($"D) {q.allAnswers[3]} {(q.correctAnswerIndex == 3 ? "(CORRECT)" : "")}");
            }
        }
        else
        {
            Debug.LogError("Failed to load questions");
        }
    }

    public async UniTask<List<ProcessedQuestion>> LoadRandomizedQuestions()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(jsonResponse);

                if (apiResponse.response_code != 0)
                {
                    Debug.LogError($"API Error: Response code {apiResponse.response_code}");
                    return null;
                }

                List<ProcessedQuestion> randomizedQuestions = new List<ProcessedQuestion>();

                foreach (var apiQuestion in apiResponse.results)
                {
                    // Create processed question with randomized answers
                    var pq = new ProcessedQuestion
                    {
                        question = DecodeHtml(apiQuestion.question),
                        allAnswers = new string[4],
                        correctAnswerIndex = Random.Range(0, 4)
                    };

                    // Place correct answer
                    pq.allAnswers[pq.correctAnswerIndex] = DecodeHtml(apiQuestion.correct_answer);

                    // Place incorrect answers
                    for (int i = 0, wrongIndex = 0; i < 4; i++)
                    {
                        if (i != pq.correctAnswerIndex)
                        {
                            pq.allAnswers[i] = DecodeHtml(apiQuestion.incorrect_answers[wrongIndex++]);
                        }
                    }

                    randomizedQuestions.Add(pq);
                }

                return randomizedQuestions;
            }
            else
            {
                Debug.LogError($"API Error: {request.error}");
                return null;
            }
        }
    }

    private string DecodeHtml(string htmlText)
    {
        return UnityWebRequest.UnEscapeURL(htmlText)
            .Replace("&quot;", "\"")
            .Replace("&amp;", "&")
            .Replace("&#039;", "'")
            .Replace("&ldquo;", "\"")
            .Replace("&rdquo;", "\"");
    }
}