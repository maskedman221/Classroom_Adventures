using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapChecker : MonoBehaviour
{
    [Header("Tilemap Reference")]
    public Tilemap targetTilemap;
    
    [Header("Position to Check")]
    public int xPosition;
    public int yPosition;
    
    [Header("Check Settings")]
    [Tooltip("Check automatically when values change")]
    public bool autoCheck = true;
    
    [Tooltip("Manual check button")]
    public bool manualCheck = false;
    
    private void OnValidate()
    {
        if (autoCheck && targetTilemap != null)
        {
            CheckTile();
        }
    }
    
    private void Update()
    {
        if (manualCheck)
        {
            manualCheck = false; // Reset the button
            CheckTile();
        }
    }
    
    public void CheckTile()
    {
        if (targetTilemap == null)
        {
            Debug.LogError("No tilemap assigned!");
            return;
        }
        
        Vector3Int cellPosition = new Vector3Int(xPosition, yPosition, 0);
        bool hasTile = targetTilemap.HasTile(cellPosition);
        
        Debug.Log($"Tile at ({xPosition}, {yPosition}): {(hasTile ? "EXISTS" : "EMPTY")}");
        
        // Optional: Visual feedback in Scene view
        Debug.DrawLine(
            targetTilemap.CellToWorld(cellPosition),
            targetTilemap.CellToWorld(cellPosition) + Vector3.one,
            hasTile ? Color.green : Color.red,
            1f);
    }
}