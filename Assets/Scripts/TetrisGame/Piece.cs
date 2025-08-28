using UnityEngine;
using System;
public class Piece : MonoBehaviour
{   
    private Board board;
    private TetrominoData data;
    private Vector3Int[] cells;
    private Vector3Int position;
    private int rotationIndex;

   [SerializeField] private float stepDelay = 1f;
    private float moveDelay = 0.1f;
    private float lockDelay = 0.5f;

    private float stepTime;
    private float moveTime;
    private float lockTime;

    private bool leftButton=false;
    private bool rightButton=false;
    private bool rotateButton=false;
    private bool lockButton=false;
    private bool resetButton=false;
    public event EventHandler OnPieceLock;
    public void Initialize(Board board,Vector3Int position ,TetrominoData data  ){
        this.data = data;
        this.board = board;
        this.position = position;
        this.stepTime = Time.time + stepDelay;
        this.moveTime = Time.time + moveDelay;        
        this.lockTime=0.0f;
        rotationIndex = 0;
       
        cells = new Vector3Int[data.cells.Length];
       
          for (int i = 0; i < cells.Length; i++) {
                    cells[i] = (Vector3Int)data.cells[i];
            }
    }

    public void UpdateMovment(){
        this.board.Clear(this);

        lockTime += Time.deltaTime; 


         if (leftButton) {
            Move(Vector2Int.left);
            leftButton=false;
        } else if (rightButton) {
            Move(Vector2Int.right);
            rightButton=false;
        }
        if(rotateButton){
            Rotate(1);
            rotateButton=false;
        }
        if(lockButton){
            Lock();
            lockButton=false;
        }
        if(resetButton){
            board.GameOver();
            resetButton=false;
        }
        // Advance the piece to the next row every x seconds
        if (Time.time > stepTime) {
            Step();
        }

        this.board.Set(this);
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        // Step down to the next row
        Move(Vector2Int.down);

        // Once the piece has been inactive for too long it becomes locked
        if (lockTime >= lockDelay) {
            Lock();
        }
    }

    private void Lock()
    {
        board.Set(this);
        // cells=null;
        TetrisGameManager.Instance.SetPiece(null , false);
        OnPieceLock?.Invoke(this, EventArgs.Empty);
        Debug.Log(board.CheckNumberPatter('5'));
    }


    public void pushButton(int buttonKey){
        if(buttonKey==1){
            // this.board.Clear(this);
            // bool yes = Move(Vector2Int.left);
            // Debug.Log(yes);
            leftButton=true;
        }
        if(buttonKey==2){
        //     this.board.Clear(this);
        //    bool yes = Move(Vector2Int.right);
        //     Debug.Log(yes);
            rightButton=true;
            
        }
        if(buttonKey==3){
            // this.board.Clear(this);
            // Rotate(1);
            rotateButton=true;
        }
        if(buttonKey==4){
            lockButton=true;
        }

        if(buttonKey==5){
            resetButton=true;
        }
        // this.board.Set(this);
    }


    private bool Move(Vector2Int translation){
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        bool valid = board.IsValidPosition(this , newPosition);
        if(valid){
            position=newPosition;
            lockTime=0.0f;
        }
        return valid;
    }

     private void Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = rotationIndex;

        // Rotate all of the cells using a rotation matrix
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.U:
                case Tetromino.C:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

     private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation)) {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }

    public Board GetBoard(){
        return board;
    }
    public TetrominoData GetTetrominoData(){
        return data;
    }
    public Vector3Int Getposition(){
        return position;
    }
    public Vector3Int[] Getcells(){
        return cells;
    }
}
