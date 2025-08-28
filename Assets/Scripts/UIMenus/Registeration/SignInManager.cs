using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
public class SignInManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button signInButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Canvas welcomeScreen;
    [SerializeField] private Canvas signInScreen;
    [SerializeField] private Canvas selectionChildScreen;
    private void Start()
    {
        signInButton.onClick.AddListener(() => HandleSignIn().Forget());
        backButton.onClick.AddListener(() => goToWelcome());

    }


    private async UniTaskVoid HandleSignIn()
    {
        string email = emailField.text;
        string password = passwordField.text;
        bool signedIn = await LoadingManager.Instance.RunWithLoading(() => ApiRegistration.Instance.SignIn(email, password)); 
        if (signedIn)
        {
            goToSelectionChild();
        }
    }

    private void goToWelcome()
    {
        signInScreen.gameObject.SetActive(false);
        welcomeScreen.gameObject.SetActive(true);
    }
    private void goToSelectionChild()
    {
       signInScreen.gameObject.SetActive(false); 
       selectionChildScreen.gameObject.SetActive(true); 
    }
}
