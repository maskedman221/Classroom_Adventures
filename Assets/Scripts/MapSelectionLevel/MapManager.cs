using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    [SerializeField] private Player player;
    [SerializeField] private int childId = 1;
    [SerializeField] private Transform mapLevels;
    private ApiGetLoader api = new ApiGetLoader();
    public event EventHandler OnStageIdFilled;

    private void Awake()
    {
        Instance = this;
    }
    private async void Start()
    {
        childId = MapData.childId;
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
                MapData.stage_id.Add(stage.id);
                Debug.Log(MapData.stage_id[0]);
            }
            Debug.Log($"✅ Map loaded with {map.data.stages.Length} stages");
            // TODO: setup your stages/levels here
        }
        else
        {
            Debug.LogError("❌ Failed to load map.");
        }
        Player.Instance.OnDestinationReached += OnDestinationReached_OpenLesson;
        OnStageIdFilled?.Invoke(this, EventArgs.Empty);
        AssignStageIdsToLevels();
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
            Debug.Log("Hit object: " + raycastHit.collider.gameObject.name);
            Debug.Log("Clicked on level, moving player...");
            player.MoveTo(level.GetSplinePoint());
        }
    }
    private void AssignStageIdsToLevels()
{
    if (mapLevels == null)
    {
        Debug.LogError("❌ mapLevels parent is not assigned!");
        return;
    }

    // Get all LevelTargets under the parent
    LevelTarget[] targets = mapLevels.GetComponentsInChildren<LevelTarget>();

    // Sort by hierarchy order so the order is predictable
    targets = targets.OrderBy(t => t.transform.GetSiblingIndex()).ToArray();

    // Assign stage IDs
    for (int i = 0; i < targets.Length && i < MapData.stage_id.Count; i++)
    {
        targets[i].SetStageId(MapData.stage_id[i]);
        Debug.Log($"Assigned Stage ID {MapData.stage_id[i]} to {targets[i].name}");
    }
}
}
