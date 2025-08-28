using UnityEngine;
using System;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
public class ImageGameManager : MonoBehaviour
{
    public static ImageGameManager Instance {private set; get;}
    [SerializeField] private Canvas keyPadCanva;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;
    public event EventHandler onStateChanged;
    private bool enterIsPressed;
    private bool isWin=false;
    private int answer;
    private int index = 0;

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


    private void Awake(){
        Instance = this;
        state = State.WaitingToStart;
        enterIsPressed=false;
        keyPadCanva.gameObject.SetActive(false);

    }

    private async UniTaskVoid Update() {
     switch (state)
     {
        case State.WaitingToStart:
    // Wait until the stopwatch is no longer running
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
            if(gamePlayingTimer < 0f) {
                state = State.GamePlaying;
                onStateChanged?.Invoke(this, EventArgs.Empty);

            }
            break;
        case State.GamePlaying:
            keyPadCanva.gameObject.SetActive(true);
            if( enterIsPressed){
                if(isWin){
                    state=State.GameWin;
                }
                else{
                    state=State.GameOver;
                }
            }
            onStateChanged?.Invoke(this, EventArgs.Empty);

            break;
        case State.GameWin:
            Debug.Log("You win");
            break;    
        case State.GameOver:
            Debug.Log("You lose");
            break;
     }   
    }





    public void SetEnterIsPressed(bool pressed, int answer)
    {
        enterIsPressed = pressed;
        this.answer = answer;
        isWin = IsWin();
        index++;
    }
    private bool IsWin()
    {
        Debug.Log($"answer {answer}");
        if (answer == ImageGameData.answers[index])
        {
            Debug.Log($"the Anwer is { ImageGameData.answers[index]}");
            winCanvas.SetActive(true);
            return true;
        }
        loseCanvas.SetActive(true);
        return false;
    }
    public float GetGamePlayingTimerNormalize(){
        return 1 - (gamePlayingTimer / maxGamePlayingTime);
    }

    public bool GetisGamePlaying(){
        return state==State.GamePlaying;
    }
}
