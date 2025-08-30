
using UnityEngine;
using UnityEngine.UI;
using System;
public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Update(){
        
        if (ImageGameManager.Instance != null)
        {
        timerImage.fillAmount = ImageGameManager.Instance.GetGamePlayingTimerNormalized();
        }
         else if (QuizManager.Instance != null)
        {
        timerImage.fillAmount = QuizManager.Instance.GetGamePlayingTimerNormalize();
        }

    }
}
