using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }
    private List<ImageCheckSO> imageCheckSOList;


    public bool IsInitialized { get; private set; } = false;
    public Stopwatch apiCallStopwatch{get; private set;} 
    private void Awake()
    {
        Instance = this;
        imageCheckSOList = new List<ImageCheckSO>();
        apiCallStopwatch = new Stopwatch();
    }

    private async UniTaskVoid Start()
    {   
        apiCallStopwatch.Start();
        // Wait until ApiImageLoader is initialized
        while (!ApiImageLoader.Instance.IsInitialized)
        {
            await UniTask.Yield();
        }

        ImageItem[] images = ApiImageLoader.Instance.GetFixedImageItem();
        await LoadImageCheckSOList(images);
        apiCallStopwatch.Stop();
        IsInitialized = true;
    }
    private async UniTask LoadImageCheckSOList(ImageItem[] images)
    {
        var tasks = new List<UniTask>();

        for (int i = 0; i < 10; i++) // or however many you need
        {
            int index = i;
            tasks.Add(LoadAndAddImage(images[index]));
        }

        await UniTask.WhenAll(tasks);
    }

   private async UniTask LoadAndAddImage(ImageItem imageItem)
{
    ImageCheckSO imageCheckSO = new ImageCheckSO();
    imageCheckSO.sprite = await ApiImageLoader.Instance.LoadSpriteFromUrl(imageItem.download_url);
    imageCheckSO.check = true;
    imageCheckSOList.Add(imageCheckSO);
}

    public List<ImageCheckSO> GetImageCheckSOList() 
    {
        if (imageCheckSOList == null)
    {
       
        return new List<ImageCheckSO>();
    }
        return imageCheckSOList;
    }
}