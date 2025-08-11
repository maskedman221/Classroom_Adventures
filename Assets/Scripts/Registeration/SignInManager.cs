using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
public class SignInManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button signInButton;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Canvas signUpScreen;
    [SerializeField] private Canvas signInScreen;
    private void Start()
    {
        signInButton.onClick.AddListener(() => HandleSignIn().Forget());
        signUpButton.onClick.AddListener(() => goToSignUp());

    }


    private async UniTaskVoid HandleSignIn()
    {
        string email = emailField.text;
        string password = passwordField.text;
        bool signedIn = await ApiRegistration.Instance.SignIn(email, password);
    }

    private void goToSignUp()
    {
        signInScreen.gameObject.SetActive(false);
        signUpScreen.gameObject.SetActive(true);
    }
}
