using UnityEngine;

public class LevelTarget : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float splinePoint = 0.0f;


    public float GetSplinePoint()
    {
        return splinePoint;
    }
}
