
using UnityEngine;
using UnityEngine.UI;
using System;
public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Update(){
        timerImage.fillAmount = ImageGameManager.Instance.GetGamePlayingTimerNormalize();

    }
}
