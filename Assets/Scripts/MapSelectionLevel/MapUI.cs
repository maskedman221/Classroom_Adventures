using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapUI : MonoBehaviour
{
    public static MapUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerGrade;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetMapUItext(MapData.playerName, MapData.playerGrade);
    }
    public void SetMapUItext(string playerName, string playerGrade)
    {
        this.playerName.text = playerName;
        this.playerGrade.text = playerGrade;
    }
}