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
        if (IsCompleted() || handlingProblem) return;

        handlingProblem = true;

        string answer = problems[currentProblemIndex].answer;
        bool problemCompleted = true;

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

            Board.Instance.ClearAllPieces();

            currentProblemIndex++;

            if (IsCompleted())
            {
                FinishGame();
            }
            else
            {
                ShowCurrentProblem();
                state = State.WaitingToStart;
            }
        }

        handlingProblem = false;
    }

    private async void FinishGame()
    {
        if (Score.GetScore() > 0)
        {
            int? current_stage_id = await api.UpdateChildProgress(
                MapDataManager.Instance.Data.childId,
                MapDataManager.Instance.Data.order,
                Score.GetScore()
            );

            if (current_stage_id.HasValue)
            {
                // Ensure array is long enough
                if (MapDataManager.Instance.Data.current_stage_id == null)
                {
                    MapDataManager.Instance.Data.current_stage_id = new int[MapDataManager.Instance.Data.order];
                }
                else if (MapDataManager.Instance.Data.current_stage_id.Length < MapDataManager.Instance.Data.order)
                {
                    Array.Resize(ref MapDataManager.Instance.Data.current_stage_id, MapDataManager.Instance.Data.order);
                }

                int index = MapDataManager.Instance.Data.order - 1;
                if (index >= 0 && index < MapDataManager.Instance.Data.current_stage_id.Length)
                {
                    MapDataManager.Instance.Data.current_stage_id[index] = current_stage_id.Value;
                    Debug.Log("Updated current_stage_id: " + MapDataManager.Instance.Data.current_stage_id[index]);
                }
                else
                {
                    Debug.LogWarning("[TetrisGameManager] Order index out of bounds.");
                }
            }

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
