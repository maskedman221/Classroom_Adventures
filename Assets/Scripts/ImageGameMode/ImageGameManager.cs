using UnityEngine;
using System;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
public class ImageGameManager : MonoBehaviour
{
    public static ImageGameManager Instance {private set; get;}
    [SerializeField] private Canvas keyPadCanva;

    public event EventHandler onStateChanged;

    private bool enterIsPressed;
    private bool isWin=false;

    private enum State{
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





    public void SetEnterIsPressed(bool pressed , bool win){
        enterIsPressed = pressed;
        isWin = win;

    }

    public float GetGamePlayingTimerNormalize(){
        return 1 - (gamePlayingTimer / maxGamePlayingTime);
    }

    public bool GetisGamePlaying(){
        return state==State.GamePlaying;
    }
}
