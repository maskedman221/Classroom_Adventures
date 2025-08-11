using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class ApiRegistration : MonoBehaviour
{
    public static ApiRegistration Instance { get; private set; }
    private string baseUrl = "http://127.0.0.1:8000/api";
    private string endpointSignIn = "login";
    private string endpointSignUp = "register";

    [System.Serializable]
    public class RegisterationResponse
    {
        public string token;
    }

    private void Awake()
    {
        Instance = this;
    }

    public async UniTask<bool> SignIn(string email, string password)
    {
        string signInApi = $"{baseUrl}/{endpointSignIn}";

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(signInApi, form))
        {
            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                RegisterationResponse response = JsonUtility.FromJson<RegisterationResponse>(json);
                SecurePlayerPrefs.SaveEncryptedString("auth_token", response.token);
                Debug.Log("Sign-in worked: " + response.token);
                return true;
            }
            else
            {
                Debug.LogError("Sign-in failed: " + request.error);
                return false;
            }
        }
    }

    public async UniTask<bool> SignUp(string username, string email, string password, string confPassword)
    {
        string signUpApi = $"{baseUrl}/{endpointSignUp}";
        WWWForm form = new WWWForm();
        form.AddField("name", username);
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("password_confirmation", confPassword);
        using (UnityWebRequest request = UnityWebRequest.Post(signUpApi, form))
        {
            await request.SendWebRequest().ToUniTask();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                RegisterationResponse response = JsonUtility.FromJson<RegisterationResponse>(json);
                SecurePlayerPrefs.SaveEncryptedString("auth_token", response.token);
                Debug.Log("Sign-up worked: " + response.token);
                return true;
            }
            else
            {
                Debug.LogError("Sign-up failed: " + request.error);
                return false;
            }
        }
    }
}

