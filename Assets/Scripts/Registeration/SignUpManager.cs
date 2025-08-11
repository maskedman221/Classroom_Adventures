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
    [SerializeField] private Button signInButton;
    [SerializeField] private Canvas signUpScreen;
    [SerializeField] private Canvas signInScreen;

    private void Start()
    {
        signUpButton.onClick.AddListener(() => HandleSignUp().Forget());
        signInButton.onClick.AddListener(() => goToSignIn());
    }

    private async UniTaskVoid HandleSignUp()
    {
        string username = usernameField.text;
        string email = emailField.text;
        string password = passwordField.text;
        string confirPassword = confirPasswordField.text;
        bool signedUp = await ApiRegistration.Instance.SignUp(username, email, password, confirPassword);
    }
    private void goToSignIn()
    {
        signUpScreen.gameObject.SetActive(false);
        signInScreen.gameObject.SetActive(true);
    }
}
