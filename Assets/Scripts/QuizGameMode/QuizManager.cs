using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text[] answerTexts;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text scoreText;

    [Header("Quiz Settings")]
    [SerializeField] private int gameModeId = 3; // The ID of the quiz game mode to load
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;

    private List<QuizQuestionData> questions;
    private int currentQuestionIndex = 0;
    private int score = 0;
    private float maxGamePlayingTime = 10f;
    private float gamePlayingTimer;

    public ApiGetLoader api = new ApiGetLoader();
    public event EventHandler onStateChanged;

    private enum State
    {
        WaitingToStart,
        CountdownTimer,
        answerQuestion,
        GameWin,
        GameLose,
    }

    private State state;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        resultPanel.SetActive(false);
        LoadQuizData();
        ShowCurrentQuestion();
        state = State.WaitingToStart;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                state = State.CountdownTimer;
                gamePlayingTimer = maxGamePlayingTime;
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.CountdownTimer:
                Debug.Log("quiz counting");
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.answerQuestion;
                    onStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.answerQuestion:
                state = State.WaitingToStart;
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.GameWin:
                winCanvas.SetActive(true);
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.GameLose:
                loseCanvas.SetActive(true);
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    private void LoadQuizData()
    {
        if (!QuizGameData.quizGames.ContainsKey(gameModeId) || QuizGameData.quizGames[gameModeId].Count == 0)
        {
            Debug.LogError("No questions found for gameModeId: " + gameModeId);
            quizPanel.SetActive(false);
            return;
        }

        questions = new List<QuizQuestionData>(QuizGameData.quizGames[gameModeId]);
        Debug.Log(questions.Count);
    }

    private void ShowCurrentQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            Score.countGames = questions.Count;
            EndQuiz();
            return;
        }

        var currentQuestion = questions[currentQuestionIndex];

        questionText.text = currentQuestion.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerTexts[i].text = currentQuestion.choices[i];

            answerButtons[i].onClick.RemoveAllListeners();
            int answerIndex = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(answerIndex));
        }
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        var currentQuestion = questions[currentQuestionIndex];
        bool isCorrect = selectedIndex == currentQuestion.correctOption;

        if (isCorrect)
        {
            score++;
            Score.gotIt++;
            answerButtons[selectedIndex].image.color = Color.green;
        }
        else
        {
            answerButtons[selectedIndex].image.color = Color.red;
            answerButtons[currentQuestion.correctOption].image.color = Color.green;
        }

        Invoke(nameof(LoadNextQuestion), 1.5f);
        state = State.answerQuestion;
    }

    private void LoadNextQuestion()
    {
        foreach (var button in answerButtons)
        {
            button.image.color = HexToColor("#85A4A4");
        }

        currentQuestionIndex++;
        ShowCurrentQuestion();
    }

    private async void EndQuiz()
    {
        quizPanel.SetActive(false);
        resultPanel.SetActive(true);
        scoreText.text = $"{score}/{questions.Count}";

        await UniTask.Delay(1500);
        resultPanel.SetActive(false);

        int totalQuestions = questions.Count;

        // Calculate player's score percentage
        float percentage = (float)score / totalQuestions;

        // Win threshold (50% correct)
        if (percentage >= 0.5f)
        {
            int? current_stage_id = await api.UpdateChildProgress(
                MapDataManager.Instance.Data.childId,
                MapDataManager.Instance.Data.order,
                Score.GetScore()
            );

            if (current_stage_id.HasValue)
            {
                MapDataManager.Instance.Data.current_stage_id[2] = current_stage_id.Value;
                Debug.Log("Updated current_stage_id: " +
                    MapDataManager.Instance.Data.current_stage_id[MapDataManager.Instance.Data.order - 1]);
            }

            state = State.GameWin;
            onStateChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            state = State.GameLose;
            onStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void RestartQuiz()
    {
        currentQuestionIndex = 0;
        score = 0;

        foreach (var button in answerButtons)
        {
            button.image.color = HexToColor("#85A4A4");
        }

        resultPanel.SetActive(false);
        quizPanel.SetActive(true);

        LoadQuizData();
        ShowCurrentQuestion();
    }

    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        Debug.LogWarning($"Invalid hex color: {hex}");
        return Color.white;
    }

    public float GetGamePlayingTimerNormalize()
    {
        return 1 - (gamePlayingTimer / maxGamePlayingTime);
    }
}
