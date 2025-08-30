using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class ImageTable : MonoBehaviour
{
    [SerializeField] private Transform imageHolder;
    [SerializeField] private Transform imageTemplate;
    [SerializeField] private Transform table;

    private void Awake()
    {
        if (imageTemplate != null)
            imageTemplate.gameObject.SetActive(false);
    }

    private async UniTaskVoid Start()
    {
        // Wait for ImageManager to finish initialization
        while (ImageManager.Instance == null || !ImageManager.Instance.IsInitialized)
        {
            await UniTask.Yield();
        }

        // Subscribe to problem changes
        ImageManager.Instance.OnProblemChanged += RefreshTable;

        // Initial display
        RefreshTable(ImageManager.Instance.GetImageCheckSOList());
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (ImageManager.Instance != null)
        {
            ImageManager.Instance.OnProblemChanged -= RefreshTable;
        }
    }

    private void RefreshTable(List<ImageCheckSO> imageList)
    {
        if (imageHolder == null || imageTemplate == null || imageList == null) return;

        // Clear old images except template and table
        foreach (Transform child in imageHolder)
        {
            if (child == imageTemplate || child == table) continue;
            Destroy(child.gameObject);
        }

        // Populate new images
        foreach (var imageCheckSO in imageList)
        {
            Transform imageTransform = Instantiate(imageTemplate, imageHolder);
            imageTransform.gameObject.SetActive(true);

            Image imageComponent = imageTransform.GetComponentInChildren<Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = imageCheckSO.sprite;
                imageComponent.preserveAspect = true;
                imageComponent.type = Image.Type.Simple;
            }
            else
            {
                Debug.LogWarning("Image template has no Image component in children");
            }
        }
    }
}
