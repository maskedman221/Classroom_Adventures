using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }
    private List<ImageCheckSO> imageCheckSOList;
    private ApiImageLoader apiImageLoader = new ApiImageLoader();
    private int numberofGames;
    // private int index=0;
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
        // // Wait until ApiImageLoader is initialized
        // while (!ApiImageLoader.Instance.IsInitialized)
        // {
        //     await UniTask.Yield();
        // }

        string[] images = ImageGameData.AllImages[0];
        await LoadImageCheckSOList(images);
        apiCallStopwatch.Stop();
        IsInitialized = true;
        
    }
    private async UniTask LoadImageCheckSOList(string[] images)
    {
        var tasks = new List<UniTask>();
        if (images == null || images.Length == 0) return;
        for (int i = 0; i < images.Length; i++)
        {
            int index = i; // capture for async lambda
            tasks.Add(LoadAndAddImage(images[index]));
        }

        await UniTask.WhenAll(tasks);        
    }


   private async UniTask LoadAndAddImage(string imageItem)
{
    ImageCheckSO imageCheckSO = new ImageCheckSO();
    imageCheckSO.sprite = await apiImageLoader.LoadSpriteFromUrl(imageItem);
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