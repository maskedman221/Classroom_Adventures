using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Collections;
using System;// This provides IEnumerator

// [DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public static Board Instance { private set; get; }
    private Tilemap tilemap;
    private Piece activePiece ;
    public event EventHandler OnGameOver;
    [SerializeField] private TetrominoData[] tetrominoes;
    private Vector2Int boardSize = new Vector2Int(8, 12);
    [SerializeField] private Vector3Int spawnPosition = new Vector3Int(-1, 4, 0);
   
public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        Instance = this;
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    public void SpawnPiece(int tetrominoesIndex){
        TetrominoData data = tetrominoes[tetrominoesIndex];
        activePiece.Initialize(this, spawnPosition, data);
        Set(this.activePiece);
       
        if (IsValidPosition(activePiece, spawnPosition)) {
            Set(activePiece);
        } else {
            // GameOver();
        }
        TetrisGameManager.Instance.SetPiece(activePiece,true);
    }

    public void GameOver()
    {
        var renderer = tilemap.GetComponent<TilemapRenderer>();
        renderer.enabled = false;

        tilemap.ClearAllTiles();
        tilemap.CompressBounds();

        renderer.enabled = true;
        OnGameOver?.Invoke(this, EventArgs.Empty);
    }

    public void Set(Piece piece) {
        Vector3Int[] cells= piece.Getcells();
    for (int i = 0; i < piece.Getcells().Length; i++) {
        Vector3Int tilePosition = cells[i] + piece.Getposition();
        tilemap.SetTile(tilePosition, piece.GetTetrominoData().tile);
    }
}
        public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Getcells().Length; i++) {
            Vector3Int tilePosition = piece.Getcells()[i] + piece.Getposition();
            tilemap.SetTile(tilePosition, null);
        }
    }
    public void ClearBoardInstantly()
    {
        if (tilemap == null) return;

        // Remove all tiles from the tilemap
        tilemap.ClearAllTiles();

        // Reset active piece reference
        if (activePiece != null)
        {
            activePiece = null;
        }

        // Optionally compress the bounds
        tilemap.CompressBounds();

        Debug.Log("Board cleared instantly.");
    }
    public void ClearAllPieces()
    {
        tilemap.ClearAllTiles();
    }
    
   public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.Getcells().Length; i++)
        {
            // Calculate the position in Unity's tile grid (scaled by 0.5)
            Vector3Int tilePosition = piece.Getcells()[i] + position;

            // Check bounds (scaled for 0.5 cell size)
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                Debug.Log($"Out of bounds at: {tilePosition}");
                return false;
            }

            if (tilemap.HasTile(tilePosition))
            {
                Debug.Log($"Tile exists at: {tilePosition}");
                return false;
            }

        }
        return true;
    }

public bool CheckNumberPatter(char number){
    RectInt bounds = Bounds;
    int row = bounds.yMin+4;
    int column = bounds.xMin;
    int[] numberPattern = Data.NumberPatterns[number];
    

    Debug.Log("Row is:"  +row );
    Debug.Log("column is:"+column );
    
    if(IsThereNumber(row)){
        Debug.Log("checking number pattern");
        bool checkRow=false;
        for(int startY = row ; startY > row - 5 ; startY--){
            int mask = numberPattern[row - startY];
            
            for(int startX = column ; startX < column+7 ; startX++){

                for(int check = startX ; check < column+3 ; check++){
                    bool shouldHaveTile = (mask & (1 << (2-(check - startX )))) != 0;
                    Debug.Log(mask);
                    bool hasTile = tilemap.HasTile(new Vector3Int(check , startY , 0));
                    Debug.Log(check + " " +startY + hasTile);
                    if (shouldHaveTile != hasTile){
                        Debug.Log("Broke" + hasTile + shouldHaveTile);
                        checkRow=false;
                        
                        break;
                    }
                    
                    checkRow=true;
                }

                if(checkRow){
                    // Debug.Log("checkRow is true in row:"  + startY );
                    column = startX;
                    break;
                }
            }
        }
        if(checkRow){
            return true;
        }
    }
    return false;
}

public bool IsThereNumber(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (tilemap.HasTile(position)) {
                // row++;
                // col=bounds.xMin;
                return true;
            }
            // if(row == -2){
            //     return true;
            // }
        }

        return false;
    }
 
}