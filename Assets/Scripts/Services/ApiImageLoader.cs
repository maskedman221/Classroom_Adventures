using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class ImageItem
{
    public string download_url;
}

public class ApiImageLoader
{
    private string apiUrl = "http://localhost:8000/storage/";
    private ImageItem[] correctImages;
    private ImageItem[] wrongImages;
    
    // Add this property to check if loading is complete
    public bool IsInitialized { get; private set; } = false;

    // private void Awake()
    // {
    //     Instance = this;
    // }

    // private async UniTaskVoid Start()
    // {
    //     await FetchImageUrl();
    //     IsInitialized = true; // Mark as initialized when done
    // }

    // private async UniTask FetchImageUrl()
    // {
    //     using UnityWebRequest request = UnityWebRequest.Get(apiUrl);
    //     await request.SendWebRequest().ToUniTask();

    //     if (request.result == UnityWebRequest.Result.Success)
    //     {
    //         string json = request.downloadHandler.text;
    //         images = JsonHelper.FromJson<ImageItem>(json);
    //     }
    //     else
    //     {
    //         Debug.LogError("Failed to fetch image list: " + request.error);
    //     }
    // }
    // public void SetCorrectImages()
    // {
    //     this.correctImages = correctImages;
    // }
    public ImageItem[] GetFixedImageItemCorrect()
    {
        return correctImages;
    }
    public ImageItem[] GetFixedImageItemWrong()
    {
        return wrongImages;
    }
    
    public async UniTask<Sprite> LoadSpriteFromUrl(string url)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(apiUrl+url);
        await request.SendWebRequest().ToUniTask();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Image download failed: " + request.error);
            return null;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        return Sprite.Create(texture, rect, pivot);
    }
}