using UnityEngine;

static class LoadingBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (LoadingManager.Instance != null) return;

        var prefab = Resources.Load<LoadingManager>("LoadingManager");
        if (prefab != null)
            Object.Instantiate(prefab);
        else
            Debug.LogError("LoadingBootstrap: Could not find LoadingManager prefab in Resources/");
    }
}
