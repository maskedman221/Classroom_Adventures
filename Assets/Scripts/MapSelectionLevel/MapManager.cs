using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.UI;
public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    [SerializeField] private Player player;
    [SerializeField] private int childId = 1;
    [SerializeField] private Transform mapLevels;
    [SerializeField] private Button exitButton;
    private ApiGetLoader api = new ApiGetLoader();
    public event EventHandler OnStageIdFilled;

    private void Awake()
    {
        Instance = this;
    }
    private async void Start()
    {
        childId = MapDataManager.Instance.Data.childId;
        if (LoadingManager.Instance == null)
        {
            Debug.LogError("❌ LoadingManager.Instance is null!");
            return;
        }
        var map = await LoadingManager.Instance.RunWithLoading(() => api.GetMap(childId));

        if (map != null)
        {
            foreach (var stage in map.data.stages)
            {
                MapDataManager.Instance.Data.stages.Add(stage);
                Debug.Log(MapDataManager.Instance.Data.stages[0].id);
            }
            Debug.Log($"✅ Map loaded with {map.data.stages.Length} stages");
        }
        else
        {
            Debug.LogError("❌ Failed to load map.");
        }
        Player.Instance.OnDestinationReached += OnDestinationReached_OpenLesson;
        OnStageIdFilled?.Invoke(this, EventArgs.Empty);
        AssignStageIdsToLevels();
        exitButton.onClick.AddListener(() => ExitMap());
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick(Input.mousePosition);
        }

        // 2. Handle touch (Mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleClick(Input.GetTouch(0).position);
        }
    }
    private async void OnDestinationReached_OpenLesson(object sender, EventArgs e)
    {
        await UniTask.Delay(1000);
        SceneManager.LoadScene(4);
    }
    private void HandleClick(Vector2 touchPostion)
    {
        Vector2 worldSpace = Camera.main.ScreenToWorldPoint(touchPostion);
        RaycastHit2D raycastHit = Physics2D.Raycast(worldSpace, Vector2.zero);

        if (raycastHit.collider != null)
        {
            LevelTarget level = raycastHit.collider.GetComponent<LevelTarget>();
            Debug.Log("Current stage IDs: " + string.Join(", ", MapDataManager.Instance.Data.current_stage_id));
            if (MapDataManager.Instance.Data.current_stage_id.Any(stageId => stageId == level.GetOrder()))
            {
                Debug.Log("Hit object: " + raycastHit.collider.gameObject.name);
                Debug.Log("Clicked on level, moving player...");
                player.MoveTo(level.GetSplinePoint());
            }
            else
            {
                Debug.Log("Unable To access this Stage");
            }
        }
    }
    private void AssignStageIdsToLevels()
    {
        if (mapLevels == null)
        {
            Debug.LogError("❌ mapLevels parent is not assigned!");
            return;
        }
        LevelTarget[] targets = mapLevels.GetComponentsInChildren<LevelTarget>();
        targets = targets.OrderBy(t => t.transform.GetSiblingIndex()).ToArray();
        for (int i = 0; i < targets.Length && i < MapDataManager.Instance.Data.stages.Count; i++)
        {
            targets[i].SetStageId(MapDataManager.Instance.Data.stages[i].id);
            targets[i].SetStageOrder(MapDataManager.Instance.Data.stages[i].order);
            Debug.Log($"Assigned Stage ID {MapDataManager.Instance.Data.stages[i].id} to {targets[i].name}");
        }
    }
    private void ExitMap()
    {
        Debug.Log("Exit button clicked ✅");
        MapDataManager.Instance.SetChildId(0);
        SceneManager.LoadScene(2);
    }
}
