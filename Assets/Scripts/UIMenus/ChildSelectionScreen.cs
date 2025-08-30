using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using TMPro;
public class ChildSelectionScreen : MonoBehaviour
{
    [SerializeField] private GameObject cardTemplate;
    [SerializeField] private Transform parentCanvas;
    [SerializeField] private Canvas selectionChildScreen;
    [SerializeField] private Canvas createChildAccountScreen;
    [SerializeField] private Button createButton;
    private ApiGetLoader api = new ApiGetLoader();

    private async UniTaskVoid Start()
    {
        createButton.onClick.AddListener(() => goToCreateChildAccount());
        var response = await LoadingManager.Instance.RunWithLoading(() => api.GetChildren());
        if (response != null && response.data != null)
        {
            foreach (var child in response.data)
            {
                Sprite avatar = ImageSaver.LoadSpriteFromFile($"avatar_{child.id}.png");
                SpawnCard(child.id, child.name, child.grade, avatar, child.current_stage_id);
            }
        }
    }
    public void SpawnCard(int childId, string name, string grade_level, Sprite icon, int current_stage_id)
    {
        // 1. Instantiate prefab
        GameObject card = Instantiate(cardTemplate, parentCanvas);

        // 2. Get references to children
        Transform emptyChild = card.transform.GetChild(0); // the "Empty" child
        TMP_Text nameText = emptyChild.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text grade_levelText = emptyChild.GetChild(2).GetComponent<TMP_Text>();
        Image iconImage = emptyChild.GetChild(0).GetComponent<Image>();

        // 3. Set values
        nameText.text = name;
        grade_levelText.text = grade_level;
        iconImage.sprite = icon;
        card.gameObject.SetActive(true);
        Button playButton = emptyChild.GetChild(3).GetChild(0).GetComponent<Button>();

        // 5. Hook listener
        playButton.onClick.AddListener(() =>
        {
                    int[] stageArray;
            if (current_stage_id == 1)
                stageArray = new int[] { 1 };
            else if (current_stage_id == 2)
                stageArray = new int[] { 1, 2, 2 };
            else if (current_stage_id == 3)
                stageArray = new int[] { 1, 2, 3 };
            else
                stageArray = new int[] { };

            MapDataManager.Instance.Data.current_stage_id = stageArray;
            MapDataManager.Instance.Data.playerName = name;
            MapDataManager.Instance.Data.playerGrade = grade_level;
            MapDataManager.Instance.Data.childId = childId;
            MapDataManager.Instance.Data.openSelectionChild = false;
            SceneManager.LoadScene(3); // or use build index
        });
    }
    private void goToCreateChildAccount()
    {
        selectionChildScreen.gameObject.SetActive(false);
        createChildAccountScreen.gameObject.SetActive(true);
    }
    // private void goToMapScreen()
    // {
    //     MapDataManager.Instance.Data.playerName = 
    //     SceneManager.LoadScene(3); 
    // }
}
