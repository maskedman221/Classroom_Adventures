using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
public class KeypadController : MonoBehaviour
{
    public  static KeypadController Instance { private set; get; }
    [SerializeField] private TMP_InputField displayInputField; // Assign in Inspector
    private int numberOfImages;
    private int answer;
    private void Awake()
    {
        Instance = this;
    }
    public void AppendKey(string key)
    {
        if (key == "BACK")
        {
            if (displayInputField.text.Length > 0)
            {
                displayInputField.text = displayInputField.text.Remove(displayInputField.text.Length - 1);
            }
        }
        else if (key == "ENTER")
        {
            if (int.TryParse(GetInputText(), out answer))
            {
                numberOfImages = ImageManager.Instance.GetImageCheckSOList().Count;
                Debug.Log(answer);
                Debug.Log(numberOfImages);
                
                ImageGameManager.Instance.SetEnterIsPressed(true, answer);
                
            }
            else
            {
                Debug.LogWarning("Invalid input! Please enter a number.");
                // Optionally reset input or show an error message
                displayInputField.text = "";
            }
        }
        else
        {
            displayInputField.text += key;
        }
    }
    // Add to KeypadController.cs

    
public void ButtonPressed(Button button)
    {
        StartCoroutine(ButtonAnimation(button));
    }

private IEnumerator ButtonAnimation(Button button)
{
    button.transform.localScale = Vector3.one * 0.9f;
    yield return new WaitForSeconds(0.1f);
    button.transform.localScale = Vector3.one;
}

private void ValidateInput()
{
    string cleanInput = new string(displayInputField.text.Where(char.IsDigit).ToArray());
    displayInputField.text = cleanInput;
}

public string GetInputText()
    {
        if (displayInputField != null)
        {
            return displayInputField.text;
        }
        else
        {
            Debug.LogWarning("InputField reference is missing!");
            return string.Empty;
        }
    }
}