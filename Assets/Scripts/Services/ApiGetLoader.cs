using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
//using Newtonsoft.Json;  // <-- Add this

public class ApiGetLoader
{
    private string baseUrl = "http://127.0.0.1:8000/api";
    private string endpointGetChildren = "children"; // check casing!
    private string endpointGetMap = "map";
    private string endpointGetStage = "stages";
    private string endpointGetStageContent = "stage-content";
    public async UniTask<RootResponse> GetChildren()
    {
        string url = $"{baseUrl}/{endpointGetChildren}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // ✅ Add Authorization if needed
            string token = SecurePlayerPrefs.GetEncryptedString("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", $"Bearer {token}");
            }

            // Send request async
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

                // ✅ Deserialize with Newtonsoft
                RootResponse response = JsonUtility.FromJson<RootResponse>(json);
                return response;
            }
            else
            {
                Debug.LogError("GetChildren failed: " + request.error);
                return null;
            }
        }
    }
    public async UniTask<RootResponseMap> GetMap(int childId)
    {
        string url = $"{baseUrl}/{endpointGetChildren}/{childId}/{endpointGetMap}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // ✅ Add Authorization if needed
            string token = SecurePlayerPrefs.GetEncryptedString("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", $"Bearer {token}");
            }

            // Send request async
            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text.Trim();
                try
                {
                    var parsed = JsonUtility.FromJson<RootResponseMap>(json);
                    string prettyJson = JsonUtility.ToJson(parsed, true); // true = pretty print
                    Debug.Log("Response (JSON):\n" + prettyJson);
                }
                catch
                {
                    Debug.Log("Response (raw):\n" + json);
                }

                // ✅ Deserialize with JsonUtility
                RootResponseMap response = JsonUtility.FromJson<RootResponseMap>(json);

                // Example: log stage IDs
                if (response?.data?.stages != null)
                {
                    foreach (var stage in response.data.stages)
                    {
                        Debug.Log($"Stage ID: {stage.id}");
                    }
                }

                return response;
            }
            else
            {
                Debug.LogError("GetMap failed: " + request.error);
                return null;
            }
        }
    }
    public async UniTask<RootResponseStage> GetStage(int stageId)
    {
        string url = $"{baseUrl}/{endpointGetStage}/{stageId}/{endpointGetStageContent}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            string token = SecurePlayerPrefs.GetEncryptedString("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", $"Bearer {token}");
            }
            await request.SendWebRequest().ToUniTask();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text.Trim();
                try
                {
                    var parsed = JsonUtility.FromJson<RootResponseStage>(json);
                    string prettyJson = JsonUtility.ToJson(parsed, true); // true = pretty print
                    Debug.Log("Response (JSON):\n" + prettyJson);
                }
                catch
                {
                    Debug.Log("Response (raw):\n" + json);
                }
                RootResponseStage response = JsonUtility.FromJson<RootResponseStage>(json);
                return response;
            }
             else
            {
                Debug.LogError("GetStage failed: " + request.error);
                return null;
            }
            
        }
       
    }
 }