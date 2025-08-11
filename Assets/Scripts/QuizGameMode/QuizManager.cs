using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    // UI References (TMP versions)
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text[] answerTexts; // Assign 4 TMP Text components
    [SerializeField] private Button[] answerButtons;  // Assign 4 Buttons
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text scoreText;

    private List<ApiQuestionLoader.ProcessedQuestion> questions;
    private int currentQuestionIndex = 0;
    private int score = 0;

    private async void Start()
    {   
        resultPanel.SetActive(false);
        await InitializeQuiz();
        ShowCurrentQuestion();
    }

    private async UniTask InitializeQuiz()
    {
        questions = await ApiQuestionLoader.Instance.LoadRandomizedQuestions();
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("Failed to load questions");
            quizPanel.SetActive(false);
            return;
        }
    }

    private void ShowCurrentQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndQuiz();
            return;
        }

        var currentQuestion = questions[currentQuestionIndex];
        
        // Set question text (TMP)
        questionText.text = currentQuestion.question;
        
        // Set answer texts (TMP) and button handlers
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerTexts[i].text = currentQuestion.allAnswers[i];
            
            // Clear previous listeners and add new one
            answerButtons[i].onClick.RemoveAllListeners();
            int answerIndex = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(answerIndex));
        }
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        bool isCorrect = selectedIndex == questions[currentQuestionIndex].correctAnswerIndex;
        
        if (isCorrect)
        {
            score++;
            Debug.Log("Correct!");
            // Visual feedback (e.g., highlight correct answer green)
            answerButtons[selectedIndex].image.color = Color.green;
        }
        else
        {
            Debug.Log("Incorrect!");
            // Visual feedback (highlight wrong answer red and correct answer green)
            answerButtons[selectedIndex].image.color = Color.red;
            answerButtons[questions[currentQuestionIndex].correctAnswerIndex].image.color = Color.green;
        }

        // Move to next question after delay
        Invoke(nameof(LoadNextQuestion), 1.5f);
    }

    private void LoadNextQuestion()
    {
        // Reset button colors
        foreach (var button in answerButtons)
        {
            button.image.color = HexToColor("#85A4A4");
        }

        currentQuestionIndex++;
        ShowCurrentQuestion();
    }

    private void EndQuiz()
    {
        quizPanel.SetActive(false);
        resultPanel.SetActive(true);
        scoreText.text = $"{score}/{questions.Count}";
    }

    public async void RestartQuiz()
    {
        currentQuestionIndex = 0;
        score = 0;
        
        // Reset UI
        foreach (var button in answerButtons)
        {
            button.image.color = HexToColor("#85A4A4");
        }
        
        resultPanel.SetActive(false);
        quizPanel.SetActive(true);
        
        await InitializeQuiz();
        ShowCurrentQuestion();
    }

    private Color HexToColor(string hex)
{
    Color color = Color.white;
    if (ColorUtility.TryParseHtmlString(hex, out color))
    {
        return color;
    }
    Debug.LogWarning($"Invalid hex color: {hex}");
    return Color.white; // fallback
}
}