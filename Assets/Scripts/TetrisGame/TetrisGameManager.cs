using UnityEngine;
using System;
using TMPro;
using Cysharp.Threading.Tasks;

public class TetrisGameManager : MonoBehaviour
{
    public static TetrisGameManager Instance { get; private set; }
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;
    public event EventHandler onStateChanged;
    private bool pieceSpawned = false;
    [SerializeField] private TextMeshProUGUI question;
    private string answer;
    private Piece piece;
    private enum State
    {
        pieceIsFalling,
        noPiece,
        win,
        lose
    }

    private State state;

    private void Awake()
    {
        Instance = this;
        state = State.noPiece;
    }
    private void Start()
    {
        if (MathGameData.mathGames.TryGetValue(1, out MathProblemData mathData))
        {
            question.text = mathData.question;
            answer = mathData.answer;
        }
        Board.Instance.OnGameOver += Lose;
    }
    private void Update()
    {
        switch (state)
        {
            case State.noPiece:
                if (pieceSpawned)
                    state = State.pieceIsFalling;
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case State.pieceIsFalling:
                piece.UpdateMovment();
                if (!pieceSpawned)
                    state = State.noPiece;
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;
            case State.win:

                break;
            case State.lose:

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
    private void HandlePieceLocked(object sender, EventArgs e)
    {
    foreach (char c in answer)
    {
        if (Board.Instance.CheckNumberPatter(c))
        {
            Won();
            return;
        }
    }
    }
    private async void Won()
    {
        state = State.win;
        if (!string.IsNullOrEmpty(question.text) && !string.IsNullOrEmpty(answer))
        {
        // If last character is '?', replace it
        if (question.text.EndsWith("?"))
        {
            question.text = question.text.Substring(0, question.text.Length - 1) + answer;
        }
        else
        {
            // fallback: just append
            question.text += answer;
        }
        }
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        winCanvas.SetActive(true);
        onStateChanged?.Invoke(this, EventArgs.Empty);
    }
    private void Lose(object sender, EventArgs e)
    {
        state = State.lose;
        loseCanvas.SetActive(true);
        onStateChanged?.Invoke(this, EventArgs.Empty);
    } 
}