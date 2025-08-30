using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // needed for Button

public class ExitButton : MonoBehaviour
{
    [SerializeField] private int sceneIndex = 3;

    public void OnClick()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}