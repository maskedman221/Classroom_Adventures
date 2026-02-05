using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using System;

public class ApiRegistration : MonoBehaviour
{
    public static ApiRegistration Instance { get; private set; }

    private string baseUrl = "http://127.0.0.1:8000/api";
    private string endpointSignIn = "login";
    private string endpointSignUp = "register";
    private string endpointPostChildren = "children";

    [System.Serializable]
    public class RegistrationResponse
    {
        public string token;
    }

    [System.Serializable]
    public class ErrorResponse
    {
        public string message;
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
            string json = request.downloadHandler.text;

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    RegistrationResponse response = JsonUtility.FromJson<RegistrationResponse>(json);
                    SecurePlayerPrefs.SaveEncryptedString("auth_token", response.token);
                    Debug.Log("Sign-in successful: " + response.token);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse success JSON: " + e.Message);
                    return false;
                }
            }
            else
            {
                Debug.LogError($"Sign-in failed: {request.error}");
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(json);
                        Debug.LogError("Server message: " + error.message);
                    }
                    catch
                    {
                        Debug.LogError("Server response (raw): " + json);
                    }
                }
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
            string json = request.downloadHandler.text;

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    RegistrationResponse response = JsonUtility.FromJson<RegistrationResponse>(json);
                    SecurePlayerPrefs.SaveEncryptedString("auth_token", response.token);
                    Debug.Log("Sign-up successful: " + response.token);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse success JSON: " + e.Message);
                    return false;
                }
            }
            else
            {
                Debug.LogError($"Sign-up failed: {request.error}");
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(json);
                        Debug.LogError("Server message: " + error.message);
                    }
                    catch
                    {
                        Debug.LogError("Server response (raw): " + json);
                    }
                }
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
                request.SetRequestHeader("Authorization", $"Bearer {token}");
            else
                Debug.LogWarning("No auth token found, request may fail.");

            await request.SendWebRequest().ToUniTask();
            string json = request.downloadHandler.text;

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Player created successfully.");
                try
                {
                    var data = JSON.Parse(json); // Using SimpleJSON
                    Debug.Log("Response (JSON):\n" + data.ToString()); // <- fixed
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
                if (!string.IsNullOrEmpty(json))
                    Debug.LogError("Server response: " + json);
                return false;
            }
        }
    }
}
