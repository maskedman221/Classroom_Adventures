using UnityEngine;
using System;

public class TetrisGameManager : MonoBehaviour{

    public static TetrisGameManager Instance {get; private set;}
    public event EventHandler onStateChanged;
    private bool pieceSpawned=false;
    private Piece piece;
    private enum State{
        pieceIsFalling,
        noPiece,
    }

    private State state;

    private void Awake(){
        Instance=this;
        state = State.noPiece;
    }

    private void Update(){
        switch(state){
            case State.noPiece:
                if(pieceSpawned)
                    state=State.pieceIsFalling;
                onStateChanged?.Invoke(this, EventArgs.Empty);    
                break;
            
            case State.pieceIsFalling:
                piece.UpdateMovment();
                if(!pieceSpawned)
                    state=State.noPiece;
                onStateChanged?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    public void SetPiece(Piece piece , bool pieceSpawned){
        this.piece=piece;
        this.pieceSpawned=pieceSpawned;
    }

}