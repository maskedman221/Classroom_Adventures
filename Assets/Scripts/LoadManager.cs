using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System;
[DefaultExecutionOrder(-1000)]
public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }

    [SerializeField] private LoadingUI loadingUIPrefab;

    private LoadingUI ui;

    void Awake()
    {
        Debug.Log("LoadingManager Awake called");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Spawn UI at startup, keep it hidden
        ui = Instantiate(loadingUIPrefab, transform);
        ui.gameObject.SetActive(false);
    }

    public void Show(float progress = 0f)
    {
        ui.gameObject.SetActive(true);
        ui.SetProgress(progress);
    }

    public async UniTask Hide(float delay = 0.2f)
    {
        if (delay > 0) await UniTask.Delay(TimeSpan.FromSeconds(delay));
        ui.gameObject.SetActive(false);
    }

    public void SetProgress(float progress)
    {
        if (ui.gameObject.activeSelf)
            ui.SetProgress(progress);
    }

    /// Run any async task wrapped with loading overlay
public async UniTask<T> RunWithLoading<T>(Func<UniTask<T>> taskFunc)
{
    Show(0.05f);

    T result = await taskFunc(); // run your API task

    SetProgress(1f);
    await Hide(0.2f);

    return result;
}


    /// Simple GET request with loading overlay
    public async UniTask<string> GetJson(string url)
    {
        Show(0.05f);

        using (var req = UnityWebRequest.Get(url))
        {
            await req.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isHttpError || req.isNetworkError)
#endif
            {
                Debug.LogError($"GET {url} failed: {req.error}");
                await Hide();
                return null;
            }

            SetProgress(1f);
            await Hide(0.2f);
            return req.downloadHandler.text;
        }
    }
}
