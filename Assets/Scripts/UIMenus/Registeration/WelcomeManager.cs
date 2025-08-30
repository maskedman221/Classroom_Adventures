using UnityEngine;
using UnityEngine.UI;
public class WelcomeManager : MonoBehaviour
{
    [SerializeField] private Canvas welcomScreen;
    [SerializeField] private Canvas signInScreen;
    [SerializeField] private Canvas signUpScreen;
    [SerializeField] private Canvas selectionChildScreen;
    [SerializeField] private Button signInButton;
    [SerializeField] private Button createAnAccountButton;

    public void Start()
    {
        createAnAccountButton.onClick.AddListener(() => goToSignUp());
        signInButton.onClick.AddListener(() => goToSignIn());
        if (MapDataManager.Instance.Data.openSelectionChild)
        {
            welcomScreen.gameObject.SetActive(false);
            selectionChildScreen.gameObject.SetActive(true);
        }
    }
    private void goToSignUp()
    {
        signInScreen.gameObject.SetActive(false);
        welcomScreen.gameObject.SetActive(false);
        signUpScreen.gameObject.SetActive(true);
    }
    private void goToSignIn()
    {
        welcomScreen.gameObject.SetActive(false);
        signUpScreen.gameObject.SetActive(false);
        signInScreen.gameObject.SetActive(true);
    }
}