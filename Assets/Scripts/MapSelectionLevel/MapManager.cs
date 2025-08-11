using UnityEngine;

public class MapManager : MonoBehaviour
{
     [SerializeField] private Player player;

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
}
