using UnityEngine;
using UnityEngine.Splines;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private SplineContainer splineContainer;
    // [Range(0f, 1f)][SerializeField] private float t = 0f;
    [SerializeField] private float speed = 0.2f;
    private float currentTarget;
    private float target;
    private bool isMoving = false;
    public event EventHandler OnDestinationReached;
    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!isMoving || splineContainer == null) return; 
        
        currentTarget = Mathf.MoveTowards(currentTarget, target, speed * Time.deltaTime);
        Vector3 pos = splineContainer.EvaluatePosition(currentTarget);
        pos.z = 0;
        transform.position = pos;

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
