using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug; // Explicitly specify which Debug to use

public class LoaderScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image loadingBar;
    [SerializeField] private string sceneName;
    
    [Header("Timing Settings")]
    [SerializeField] private float minLoadingTime = 3f;
    [SerializeField] private float maxLoadingTime = 30f;

    private Stopwatch loadingStopwatch = new Stopwatch();
    private AsyncOperation sceneLoadOperation;
    private float apiLoadProgress;
    private float imageLoadProgress;
    private bool allDependenciesReady;

    private async UniTaskVoid Start()
    {
        loadingStopwatch.Start();
        
        // Start all loading operations
        var apiLoadingTask = LoadApiImages();
        var sceneLoadingTask = LoadSceneAsync();
        
        // Update progress until everything is ready
        while (loadingStopwatch.Elapsed.TotalSeconds < maxLoadingTime)
        {
            UpdateProgressUI();
            
            if (loadingStopwatch.Elapsed.TotalSeconds >= minLoadingTime && allDependenciesReady)
            {
                break;
            }
            
            await UniTask.Yield();
        }
        
        // Final activation
        if (allDependenciesReady)
        {
            sceneLoadOperation.allowSceneActivation = true;
            await sceneLoadOperation;
            SceneManager.UnloadSceneAsync(gameObject.scene);
        }
        else
        {
            Debug.LogError("Loading failed - check network connection");
        }
    }

    private async UniTask LoadApiImages()
    {
        try
        {
            // Initialize ApiImageLoader
            var apiLoader = ApiImageLoader.Instance;
            await UniTask.WaitUntil(() => apiLoader != null && apiLoader.IsInitialized);
            
            // Initialize ImageManager
            var imgManager = ImageManager.Instance;
            await UniTask.WaitUntil(() => imgManager != null && imgManager.IsInitialized);
            
            // Track progress of image loading
            while (!imgManager.IsInitialized)
            {
                apiLoadProgress = Mathf.Lerp(apiLoadProgress, 0.5f, Time.deltaTime);
                imageLoadProgress = Mathf.Lerp(imageLoadProgress, 
                    imgManager.GetImageCheckSOList().Count / 10f, Time.deltaTime);
                await UniTask.Yield();
            }
            
            apiLoadProgress = 1f;
            imageLoadProgress = 1f;
            allDependenciesReady = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Loading failed: {e.Message}");
        }
    }

    private async UniTask LoadSceneAsync()
    {
        sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        sceneLoadOperation.allowSceneActivation = false;
        
        await UniTask.WaitUntil(() => 
            allDependenciesReady && 
            loadingStopwatch.Elapsed.TotalSeconds >= minLoadingTime
        );
    }

    private void UpdateProgressUI()
    {
        try
        {
            // Scene load progress (stops at 0.9 until activation)
            float sceneProgress = sceneLoadOperation?.progress / 0.9f ?? 0f;
            
            // Combined progress (weighted average)
            float combinedProgress = (apiLoadProgress + imageLoadProgress + sceneProgress) / 3f;
            
            // Apply smooth interpolation
            float smoothProgress = Mathf.Lerp(loadingBar.fillAmount, combinedProgress, Time.deltaTime * 5f);
            
            // Ensure minimum time is respected
            float timeProgress = Mathf.Clamp01((float)(loadingStopwatch.Elapsed.TotalSeconds / minLoadingTime));
            loadingBar.fillAmount = Mathf.Max(smoothProgress, timeProgress);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Progress update error: {e.Message}");
        }
    }
}