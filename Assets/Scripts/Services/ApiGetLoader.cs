using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
//using Newtonsoft.Json;  // <-- Add this


[System.Serializable]
public class ProgressRequest
{
    public int stage_id;
    public int stars;

    public ProgressRequest(int stageId, int stars)
    {
        stage_id = stageId;
        this.stars = stars;
    }
}
public class ApiGetLoader
{
    private string baseUrl = "http://127.0.0.1:8000/api";
    private string endpointGetChildren = "children";
    private string endpointGetMap = "map";
    private string endpointGetStage = "stages";
    private string endpointGetStageContent = "stage-content";
    public async UniTask<RootResponse> GetChildren()
    {
        string url = $"{baseUrl}/{endpointGetChildren}";
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
                    var parsed = JsonUtility.FromJson<RootResponseMap>(json);
                    string prettyJson = JsonUtility.ToJson(parsed, true);
                    Debug.Log("Response (JSON):\n" + prettyJson);
                }
                catch
                {
                    Debug.Log("Response (raw):\n" + json);
                }
                RootResponseMap response = JsonUtility.FromJson<RootResponseMap>(json);

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
                    string prettyJson = JsonUtility.ToJson(parsed, true);
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
    public async UniTask<int?> UpdateChildProgress(int childId, int stageId, int stars)
    {
        string url = $"{baseUrl}/children/{childId}/progress";

        var body = new ProgressRequest(stageId, stars);
        string jsonBody = JsonUtility.ToJson(body);

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            string token = SecurePlayerPrefs.GetEncryptedString("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", $"Bearer {token}");
            }

            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text.Trim();
                Debug.Log("Update progress raw response: " + json);

                UpdateProgressResponse response = JsonUtility.FromJson<UpdateProgressResponse>(json);

                if (response != null && response.data != null)
                {
                    Debug.Log($"✅ Updated → Current Stage ID: {response.data.current_stage_id}");
                    return response.data.current_stage_id;
                }
                return null;
            }
            else
            {
                Debug.LogError("UpdateChildProgress failed: " + request.error);
                return null;
            }
        }
    }

 }
