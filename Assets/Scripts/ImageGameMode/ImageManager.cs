using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;

public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }
    [SerializeField] private TMP_Text questionField;
    public event System.Action<List<ImageCheckSO>> OnProblemChanged;

    private readonly List<ImageCheckSO> imageCheckSOList = new List<ImageCheckSO>();
    private ApiImageLoader apiImageLoader = new ApiImageLoader();

    private int currentProblemIndex = 0;
    public bool IsInitialized { get; private set; } = false;
    public Stopwatch apiCallStopwatch { get; private set; }

    [SerializeField] private int gameModeId = 1;
    private List<ImageProblemData> problems;

    public int CurrentIndex => currentProblemIndex;              // <-- for debugging/logic
    public int ProblemCount => problems?.Count ?? 0;             // <-- for debugging/logic
    public bool IsLastProblem() => currentProblemIndex == ProblemCount - 1;
    public bool IsCompleted()   => currentProblemIndex >= ProblemCount;

    private void Awake()
    {
        Instance = this;
        apiCallStopwatch = new Stopwatch();
    }

    private async UniTaskVoid Start()
    {
        apiCallStopwatch.Start();

        if (!ImageGameData.imageGames.ContainsKey(gameModeId))
        {
            UnityEngine.Debug.LogError($"GameModeId {gameModeId} not found in ImageGameData!");
            return;
        }

        problems = ImageGameData.imageGames[gameModeId];
        UnityEngine.Debug.Log($"[ImageManager] Loaded problems: {ProblemCount}");

        await ShowCurrentProblem();

        apiCallStopwatch.Stop();
        IsInitialized = true;
    }

    public async UniTask ShowCurrentProblem()
    {
        if (IsCompleted())
        {
            UnityEngine.Debug.Log("[ImageManager] All problems completed!");
            return;
        }

        imageCheckSOList.Clear();
        var problem = problems[currentProblemIndex];

        UnityEngine.Debug.Log($"[ImageManager] Show problem {currentProblemIndex + 1}/{ProblemCount}");
        if (questionField != null) questionField.text = problem.question;

        await LoadImageCheckSOList(problem.images);
        OnProblemChanged?.Invoke(imageCheckSOList);
    }

    public async UniTask LoadNextProblem()
    {
        currentProblemIndex++;
        UnityEngine.Debug.Log($"[ImageManager] Advance to index {currentProblemIndex} (count {ProblemCount})");

        if (!IsCompleted())
        {
            await ShowCurrentProblem();
        }
        else
        {
            UnityEngine.Debug.Log("[ImageManager] No more problems in this game mode!");
        }
    }

    private async UniTask LoadImageCheckSOList(string[] images)
    {
        var tasks = new List<UniTask>();
        if (images == null || images.Length == 0) return;

        for (int i = 0; i < images.Length; i++)
        {
            string imageUrl = images[i];
            tasks.Add(LoadAndAddImage(imageUrl));
        }

        await UniTask.WhenAll(tasks);
    }

    private async UniTask LoadAndAddImage(string imageItem)
    {
        var imageCheckSO = new ImageCheckSO
        {
            sprite = await apiImageLoader.LoadSpriteFromUrl(imageItem),
            check = true
        };
        imageCheckSOList.Add(imageCheckSO);
    }

    public List<ImageCheckSO> GetImageCheckSOList()
    {
        return imageCheckSOList ?? new List<ImageCheckSO>();
    }

    public ImageProblemData GetCurrentProblem()
    {
        if (currentProblemIndex >= 0 && currentProblemIndex < ProblemCount)
            return problems[currentProblemIndex];

        UnityEngine.Debug.LogWarning($"[ImageManager] GetCurrentProblem out of range: index={currentProblemIndex}, count={ProblemCount}");
        return null;
    }
}
