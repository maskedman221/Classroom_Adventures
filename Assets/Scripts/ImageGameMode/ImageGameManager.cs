using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class ImageGameManager : MonoBehaviour
{
    public static ImageGameManager Instance { private set; get; }

    [Header("UI References")]
    [SerializeField] private Canvas keyPadCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;

    [Header("Game Settings")]
    [SerializeField] private int gameModeId = 1;

    public event EventHandler onStateChanged;
    public ApiGetLoader api = new ApiGetLoader();
    private bool enterIsPressed = false;
    private bool handlingAnswer = false;     // <-- prevents double-processing while awaiting
    private int playerAnswer;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        WaitingToCountImages,
        GamePlaying,
        GameWin,
        GameOver,
    }

    private State state;
    private float maxGamePlayingTime = 10f;
    private float gamePlayingTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        state = State.WaitingToStart;
        if (keyPadCanvas != null) keyPadCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (!ImageGameData.imageGames.ContainsKey(gameModeId))
        {
            Debug.LogError($"No problems found for gameModeId {gameModeId}");
            return;
        }

        Score.countGames = ImageGameData.imageGames[gameModeId].Count;
    }

    private async UniTaskVoid Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                await UniTask.WaitWhile(() => ImageManager.Instance.apiCallStopwatch.IsRunning);
                state = State.CountdownToStart;
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.CountdownToStart:
                state = State.WaitingToCountImages;
                gamePlayingTimer = maxGamePlayingTime;
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.WaitingToCountImages:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GamePlaying;
                    if (keyPadCanvas != null) keyPadCanvas.gameObject.SetActive(true);
                    onStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.GamePlaying:
                if (enterIsPressed && !handlingAnswer)
                {
                    // prevent re-entry while we await
                    handlingAnswer = true;
                    enterIsPressed = false;

                    bool isWin = CheckAnswer();
                    if (isWin) Score.gotIt++;

                    // ✅ If we just answered the LAST problem, finish now.
                    if (ImageManager.Instance.IsLastProblem())
                    {
                        FinishGame();
                        handlingAnswer = false;
                        return;
                    }

                    // Otherwise, load the next one.
                    await ImageManager.Instance.LoadNextProblem();
                    ResetStateForNextProblem();
                    handlingAnswer = false;
                }
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.GameWin:
            case State.GameOver:
                // handled by FinishGame
                break;
        }
    }

    public void SetEnterIsPressed(bool pressed, int answer)
    {
        enterIsPressed = pressed;
        playerAnswer = answer;
    }

    private bool CheckAnswer()
    {
        var currentProblem = ImageManager.Instance.GetCurrentProblem();
        if (currentProblem == null)
        {
            Debug.LogError($"[ImageGameManager] No current problem! index={ImageManager.Instance.CurrentIndex}, count={ImageManager.Instance.ProblemCount}");
            return false;
        }

        bool correct = playerAnswer == currentProblem.correctAnswer;
        Debug.Log($"[ImageGameManager] Answer: {playerAnswer} | Correct: {currentProblem.correctAnswer} | {(correct ? "OK" : "WRONG")}");
        return correct;
    }

    private async void FinishGame()
    {
        if (keyPadCanvas != null) keyPadCanvas.gameObject.SetActive(false);

        if (Score.GetScore() > 0)  // assuming you implemented GetScore() as discussed
        {
            int? current_stage_id = await api.UpdateChildProgress(MapDataManager.Instance.Data.childId,MapDataManager.Instance.Data.order,Score.GetScore());
            if (current_stage_id.HasValue)
            {
                MapDataManager.Instance.Data.current_stage_id[0] = current_stage_id.Value;
                Debug.Log("Updated current_stage_id: " + MapDataManager.Instance.Data.current_stage_id[MapDataManager.Instance.Data.order-1]);
            }
            Debug.Log(MapDataManager.Instance.Data.current_stage_id[MapDataManager.Instance.Data.order-1]);
            if (winCanvas != null) winCanvas.SetActive(true);
            state = State.GameWin;
        }
        else
        {
            if (loseCanvas != null) loseCanvas.SetActive(true);
            state = State.GameOver;
        }

        onStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ResetStateForNextProblem()
    {
        state = State.CountdownToStart;
        gamePlayingTimer = maxGamePlayingTime;
        if (winCanvas != null)  winCanvas.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(false);
        if (keyPadCanvas != null) keyPadCanvas.gameObject.SetActive(false);
        onStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer / maxGamePlayingTime);
    }

    public bool GetIsGamePlaying()
    {
        return state == State.GamePlaying;
    }
}
