using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MapUI : MonoBehaviour
{
    public static MapUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerGrade;
    // [SerializeField] private Button exitButton;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetMapUItext(MapDataManager.Instance.Data.playerName, MapDataManager.Instance.Data.playerGrade);
        // exitButton.onClick.AddListener(() => ExitMap());
    }
    public void SetMapUItext(string playerName, string playerGrade)
    {
        this.playerName.text = playerName;
        this.playerGrade.text = playerGrade;
    }
    // private void ExitMap()
    // {
    //     Debug.Log("Exit button clicked ✅");
    //     MapDataManager.Instance.SetChildId(0);
    //     SceneManager.LoadScene(2);
    // }
}