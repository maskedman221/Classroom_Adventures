using UnityEngine;
using System;

public class LevelTarget : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float splinePoint = 0.0f;
    [SerializeField] private int stage_id;
    private int order;
    // 🔔 Event that others can subscribe to
    // public static event Action<int> OnLevelTargetClicked;
    public float GetSplinePoint() => splinePoint;
    public int GetStageId() => stage_id;
    public int GetOrder() => order;
    public void SetStageId(int id) => stage_id = id;
    public void SetStageOrder(int order) => this.order = order;

    private void OnMouseDown()
    {
        Debug.Log($"Clicked LevelTarget! Stage ID: {stage_id}");
        MapDataManager.Instance.Data.idClicked = stage_id;
        MapDataManager.Instance.Data.order = order;
    }
    // Handle touch input (Mobile)
}
