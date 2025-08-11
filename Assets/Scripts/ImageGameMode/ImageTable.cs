using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ImageTable : MonoBehaviour
{
    [SerializeField] private Transform imageHolder;
    [SerializeField] private Transform imageTemplate;
    [SerializeField] private Transform table;

    private void Awake() 
    {
        imageTemplate.gameObject.SetActive(false);
    }

    private async UniTaskVoid Start()
    {
        // Wait for ApiImageLoader to finish
        while (ApiImageLoader.Instance == null || !ApiImageLoader.Instance.IsInitialized)
        {
            await UniTask.Yield();
        }

        // Wait for ImageManager to finish
        while (ImageManager.Instance == null || !ImageManager.Instance.IsInitialized)
        {
            await UniTask.Yield();
        }
        await UniTask.DelayFrame(1);

        if (imageHolder == null || imageTemplate == null || ImageManager.Instance == null)
        {
            Debug.LogError("Essential components are not initialized!");

        }
        else
        {
            Debug.Log("pass null safty");
        }
        var imageList = ImageManager.Instance.GetImageCheckSOList();
        if (imageList == null || imageList.Count == 0)
        {
            Debug.LogWarning("No images available to display");

        }
        else
        {
            Debug.Log("pass array components exists");
        }
        UpdateVisual();
    }

    private void UpdateVisual()
    {
         if (imageHolder == null || imageTemplate == null || ImageManager.Instance == null)
    {
        Debug.LogError("Essential components are not initialized!");
        return;
    }
        foreach (Transform child in imageHolder)
        {
            if (child == imageTemplate || child == table) continue;
            Destroy(child.gameObject);
        }
         var imageList = ImageManager.Instance.GetImageCheckSOList();
    if (imageList == null || imageList.Count == 0)
    {
        Debug.LogWarning("No images available to display");
        return;
    }
        
        foreach (ImageCheckSO imageCheckSO in ImageManager.Instance.GetImageCheckSOList())
        {
            Transform imageTransform = Instantiate(imageTemplate, imageHolder);
            imageTransform.gameObject.SetActive(true);
             Image imageComponent = imageTransform.GetComponentInChildren<Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = imageCheckSO.sprite;
                
                // Optional: Adjust image display
                imageComponent.preserveAspect = true;
                imageComponent.type = Image.Type.Simple;
            }
            else
            {
                Debug.LogWarning("Hexagon prefab has no Image component in children");
            }
        }
    }
}