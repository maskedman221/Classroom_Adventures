using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class ApiRegistration : MonoBehaviour
{
    public static ApiRegistration Instance { get; private set; }
    private string baseUrl = "http://127.0.0.1:8000/api";
    private string endpointSignIn = "login";
    private string endpointSignUp = "register";
    private string endpointPostChildren = "children";

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

    public async UniTask<bool> CreatePlayerAccount(string name, string gradeLevel)
    {
        string childrenPostApi = $"{baseUrl}/{endpointPostChildren}";
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("grade_level", gradeLevel);
        using (UnityWebRequest request = UnityWebRequest.Post(childrenPostApi, form))
        {
            string token = SecurePlayerPrefs.GetEncryptedString("auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                // ✅ Add Authorization header
                request.SetRequestHeader("Authorization", $"Bearer {token}");
            }
            else
            {
                Debug.LogWarning("⚠ No token found in PlayerPrefs, request may fail.");
            }
            await request.SendWebRequest().ToUniTask();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                try
                {
                    var parsed = JsonUtility.FromJson<RootResponse>(json); 
                    string prettyJson = JsonUtility.ToJson(parsed, true); // true = pretty print
                    Debug.Log("Response (JSON):\n" + prettyJson);
                }
                catch
                {
                    Debug.Log("Response (raw):\n" + json);
                }
                return true;
            }
            else
            {
                Debug.LogError("Creating Player failed: " + request.error);
                return false;
            }
        }
    }
}

