using UnityEngine;
using System;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class TetrisGameManager : MonoBehaviour
{
    public static TetrisGameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private TextMeshProUGUI question;

    public event EventHandler onStateChanged;
    public ApiGetLoader api = new ApiGetLoader();
    private bool pieceSpawned = false;
    private Piece piece;
    private bool handlingProblem = false;
    private enum State
    {
        WaitingToStart,
        GamePlaying,
        GameWin,
        GameOver
    }

    private State state;

    // Multiple problems
    private List<MathProblemData> problems = new List<MathProblemData>();
    private int currentProblemIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        state = State.WaitingToStart;

        if (winCanvas != null) winCanvas.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(false);
    }

    private void Start()
    {
        // Load problems
        if (MathGameData.mathGamesList.TryGetValue(2, out List<MathProblemData> mathProblemList))
        {
            problems = mathProblemList;
        }
        else
        {
            Debug.LogError("[TetrisGameManager] No math problems found!");
        }

        Score.countGames = problems.Count;

        ShowCurrentProblem();

        Board.Instance.OnGameOver += Lose;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                if (pieceSpawned) state = State.GamePlaying;
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.GamePlaying:
                if (piece != null) piece.UpdateMovment();
                if (!pieceSpawned) state = State.WaitingToStart;
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.GameWin:
                break;
            case State.GameOver:
                break;
        }
    }

    public void SetPiece(Piece piece, bool pieceSpawned)
    {
        this.piece = piece;
        this.pieceSpawned = pieceSpawned;
        if (piece != null)
            piece.OnPieceLock += HandlePieceLocked;
    }

private async void HandlePieceLocked(object sender, EventArgs e)
{
    if (IsCompleted()) return;
    if (handlingProblem) return; // prevent double processing

    handlingProblem = true;

    string answer = problems[currentProblemIndex].answer;
    bool problemCompleted = true;

    // check all characters for the current problem
    foreach (char c in answer)
    {
        if (!Board.Instance.CheckNumberPatter(c))
        {
            problemCompleted = false;
            break;
        }
    }

    if (problemCompleted)
    {
        Score.gotIt++;
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        // clear the board for the next problem
        Board.Instance.ClearAllPieces();

        // move to next problem
        currentProblemIndex++;

        if (IsCompleted())
        {
            FinishGame(); // only after last problem
        }
        else
        {
            ShowCurrentProblem();
            state = State.WaitingToStart; // or a “CountdownToStart” state if needed
        }
    }

    handlingProblem = false;
}


    private async void FinishGame()
    {
        if (Score.GetScore() > 0)
        {
            int? current_stage_id = await api.UpdateChildProgress(MapDataManager.Instance.Data.childId,MapDataManager.Instance.Data.order,Score.GetScore());
            if (current_stage_id.HasValue)
            {
                MapDataManager.Instance.Data.current_stage_id[1] = current_stage_id.Value;
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

    private void Lose(object sender, EventArgs e)
    {
        state = State.GameOver;
        if (loseCanvas != null) loseCanvas.SetActive(true);
        onStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ShowCurrentProblem()
    {
        if (!IsCompleted())
        {
            question.text = problems[currentProblemIndex].question;
        }
    }

    public bool IsCompleted()
    {
        return currentProblemIndex >= problems.Count;
    }
}
