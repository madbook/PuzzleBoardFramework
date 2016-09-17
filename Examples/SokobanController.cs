﻿using UnityEngine;
using PuzzleBoardFramework;

public class SokobanController : BoardController<int> {

    const int PLAYER_TYPE = 1;
    const int CRATE_TYPE = 2;
    const int WALL_TYPE = 3;

    class SokobanStrategy : GenericMergeStrategy<int> {
        public override bool ShouldPush (int from, int into) {
            return into == CRATE_TYPE;
        }
    }

    class SokobanRenderer : BoardRenderer<int> {
        public SokobanRenderer (BaseBoard<int> board, Transform parent) : base (board, parent) {
        }

        public override GameObject CreateRenderObject () {
            return GameObject.CreatePrimitive (PrimitiveType.Cube);
        }

        public override void UpdateRenderValue (GameObject obj, int value) {
            if (value == PLAYER_TYPE) {
                obj.transform.localScale = new Vector3 (.5f, .5f, 1);
                obj.GetComponent<MeshRenderer> ().material.color = Color.red;
            } else if (value == CRATE_TYPE) {
                obj.transform.localScale = new Vector3 (1, 1, 1);
                obj.GetComponent<MeshRenderer> ().material.color = Color.yellow;
            } else {
                obj.transform.localScale = new Vector3 (1, 1, 1);
                obj.GetComponent<MeshRenderer> ().material.color = Color.gray;
            }
        }
    }

    IHistory<Record<int>> history;
    bool recordingHistory;

    public new void Start () {
        base.Start ();
        history = new History<Record<int>> ();
        Init ();
        recordingHistory = true;
    }

    public override MergeStrategy<int> GetMergeStrategy () {
        return new SokobanStrategy ();
    }

    public override BoardRenderer<int> GetBoardRenderer (BaseBoard<int> board, Transform parent) {
        return new SokobanRenderer (board, parent);
    }

    void Init () {
        board.UpdateTiles (board.GetPositionsInColumn (0), WALL_TYPE);
        board.UpdateTiles (board.GetPositionsInColumn (width - 1), WALL_TYPE);
        board.UpdateTiles (board.GetPositionsInRow (0), WALL_TYPE);
        board.UpdateTiles (board.GetPositionsInRow (height - 1), WALL_TYPE);
        board.UpdateTile (new BoardPosition (1, 1), PLAYER_TYPE);
        board.UpdateTile (new BoardPosition (2, 2), CRATE_TYPE);
        board.UpdateTile (new BoardPosition (2, 3), CRATE_TYPE);
    }

    public void Reset () {
        recordingHistory = false;
        board.Clear ();
        boardRenderer.Clear ();
        history.ClearAll ();
        Init ();
        recordingHistory = true;
    }

    public override void OnRecordReceived (Record<int> record) {
        base.OnRecordReceived (record);
        if (recordingHistory) {
            history.AddRecord (record);
        }
    }

    public void UndoLastTurn () {
        if (history.Count == 0) {
            return;
        }
        recordingHistory = false;
        foreach (Record<int> record in history.IterateLastTurn ()) {
            board.UndoRecord (record);
        }
        history.ClearLastTurn ();
        recordingHistory = true;
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.U)) {
            UndoLastTurn ();
            return;
        } else if (Input.GetKeyDown (KeyCode.R)) {
            Reset ();
            return;
        }

        MoveVector move;

        if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            move = MoveVector.left;
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            move = MoveVector.right;
        } else if (Input.GetKeyDown (KeyCode.DownArrow)) {
            move = MoveVector.down;
        } else if (Input.GetKeyDown (KeyCode.UpArrow)) {
            move = MoveVector.up;
        } else {
            move = MoveVector.zero;
        }

        if (move != MoveVector.zero) {
            board.PushAllMatching (move, PLAYER_TYPE);
            board.ApplyMovementAndReset (move);
            history.NewTurn ();
        }
    }

}