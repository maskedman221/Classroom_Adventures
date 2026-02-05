using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
public class CreatePlayer : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_Dropdown gradeDropdown;
    [SerializeField] private Button createButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button[] avatarButtons;
    [SerializeField] private Image[] avatarImages;
    [SerializeField] private GameObject selectionCanvas;
    [SerializeField] private GameObject createCanvas;
    private int selectedAvatar = 0;
    private string selectedDropdown = "Kindergarten";
    private int childId = 1;
    void Start()
    {
        createButton.onClick.AddListener(() => Handlecreation().Forget());
        backButton.onClick.AddListener(() => Back());
        gradeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            int index = i;
            avatarButtons[i].onClick.AddListener(() => OnAvatarSelected(index));
        }
        
    }
    private void Back()
    {
        selectionCanvas.SetActive(true);
        createCanvas.SetActive(false);
    }
    private async UniTaskVoid Handlecreation()
    {   
        if (selectedAvatar == null)
        {
            Debug.LogWarning("No grade selected!");
            return; // or show a message
        }
        string name = playerName.text;
        string grade_level = EscapeTMPText(selectedDropdown);
        Debug.Log(selectedDropdown);
        Sprite selectedSprite = avatarButtons[selectedAvatar].GetComponent<Image>().sprite;
        string fileName = $"avatar_{childId}.png";
        ImageSaver.SaveSpriteToFile(selectedSprite, fileName);
        bool isCreatePlayer = await ApiRegistration.Instance.CreatePlayerAccount(name, grade_level);
        selectionCanvas.SetActive(true);
        createCanvas.SetActive(false);
    }
    private void OnDropdownValueChanged(int index)
    {
        selectedDropdown = gradeDropdown.options[index].text;
        //Debug.Log(selectedDropdown);
    }
    private void OnAvatarSelected(int index)
    {
        selectedAvatar = index;
        Debug.Log(selectedAvatar);
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            Outline outline = avatarImages[i].GetComponent<Outline>();

            if (i == index)
            {
                // Set color to #f0f9ff
                Color newColor;
                ColorUtility.TryParseHtmlString("#f0f9ff", out newColor);
                avatarImages[i].color = newColor;

                // Enable outline
                if (outline != null) outline.enabled = true;
            }
            else
            {
                // Reset color to white (or whatever your default is)
                avatarImages[i].color = Color.white;

                // Disable outline
                if (outline != null) outline.enabled = false;
            }
        }
    }
        string EscapeTMPText(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return input.Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
