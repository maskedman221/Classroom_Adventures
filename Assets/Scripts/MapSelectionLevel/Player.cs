using UnityEngine;
using UnityEngine.Splines;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float speed = 0.2f;
    private float currentTarget;
    private float target;
    private bool isMoving = false;
    public event EventHandler OnDestinationReached;

    private void Awake()
    {
        Instance = this;

        // Start player at beginning of the spline and upright
        if (splineContainer != null)
        {
            Vector3 startPos = splineContainer.EvaluatePosition(0f);
            startPos.z = 0;
            transform.position = startPos;
            transform.rotation = Quaternion.identity;
            currentTarget = 0f;
            target = 0f;
        }
    }

    void Update()
    {
        if (!isMoving || splineContainer == null) return; 
        
        currentTarget = Mathf.MoveTowards(currentTarget, target, speed * Time.deltaTime);
        Vector3 pos = splineContainer.EvaluatePosition(currentTarget);
        pos.z = 0;
        transform.position = pos;

        // Keep player upright
        transform.rotation = Quaternion.identity;

        if (Mathf.Approximately(currentTarget, target))
        {
            isMoving = false;
            Debug.Log("Player reached destination");
            OnDestinationReached?.Invoke(this, EventArgs.Empty);
        }
    }

    public void MoveTo(float target)
    {
        this.target = Mathf.Clamp01(target); 
        isMoving = true;
    }
}
