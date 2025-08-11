using UnityEngine;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [Range(0f, 1f)][SerializeField] private float t = 0f;
    [SerializeField] private float speed = 0.2f;
    private float currentTarget;
    private float target;
    private bool isMoving = false;

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
        }

       
    }

    public void MoveTo(float target)
    {
        this.target = Mathf.Clamp01(target); 
        isMoving = true;
    }
}
