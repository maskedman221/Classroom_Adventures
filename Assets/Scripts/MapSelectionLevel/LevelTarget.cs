using UnityEngine;
using System;

public class LevelTarget : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float splinePoint = 0.0f;
    [SerializeField] private int stage_id;

    // 🔔 Event that others can subscribe to
    // public static event Action<int> OnLevelTargetClicked;
    public float GetSplinePoint() => splinePoint;
    public int GetStageId() => stage_id;
    public void SetStageId(int id) => stage_id = id;

    private void OnMouseDown()
    {
        Debug.Log($"Clicked LevelTarget! Stage ID: {stage_id}");
        MapData.idClicked = stage_id;
    }
    // Handle touch input (Mobile)
 


}
