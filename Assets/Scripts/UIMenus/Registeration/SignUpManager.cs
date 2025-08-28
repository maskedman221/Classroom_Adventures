using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Threading.Tasks;
public class SignUpManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField confirPasswordField;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Canvas signUpScreen;
    [SerializeField] private Canvas welcomeScreen;
    [SerializeField] private Canvas selectionChildScreen;

    private void Start()
    {
        signUpButton.onClick.AddListener(() => HandleSignUp().Forget());
        backButton.onClick.AddListener(() => goToWelcome());
    }

    private async UniTaskVoid HandleSignUp()
    {
        string username = usernameField.text;
        string email = emailField.text;
        string password = passwordField.text;
        string confirPassword = confirPasswordField.text;
        bool signedUp = await LoadingManager.Instance.RunWithLoading(() => ApiRegistration.Instance.SignUp(username, email, password, confirPassword));
        if (signedUp)
        {
            goToSelectionChild();
        }
    }
    private void goToWelcome()
    {
        signUpScreen.gameObject.SetActive(false);
        welcomeScreen.gameObject.SetActive(true);
    }
    private void goToSelectionChild()
    {
       signUpScreen.gameObject.SetActive(false); 
       selectionChildScreen.gameObject.SetActive(true); 
    }
}
