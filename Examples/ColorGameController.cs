﻿using System.Collections.Generic;
using UnityEngine;
using PuzzleBoardFramework;

public class ColorMergeStrategy : GenericMergeStrategy<Color> {
    public override bool ShouldMerge (Color from, Color into) {
        return true;
    }

    public override Color Merge (Color from, Color into) {
        return Color.Lerp (from, into, .5f);
    }
}

public class ColorRenderer : BoardRenderer<Color> {
    public ColorRenderer (BaseBoard<Color> board, Transform parent) : base (board, parent) {
    }

    public override GameObject CreateRenderObject () {
        return GameObject.CreatePrimitive (PrimitiveType.Cube);
    }

    public override void UpdateRenderValue (GameObject obj, Color value) {
        obj.GetComponent<MeshRenderer> ().material.color = value;
    }
}

public class ColorGameController : BoardController<Color> {
    public override IMergeStrategy<Color> GetMergeStrategy () {
        return new ColorMergeStrategy ();
    }

    public override BoardRenderer<Color> GetBoardRenderer (BaseBoard<Color> board, Transform parent) {
        return new ColorRenderer (board, parent);
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            board.PushAll (MoveVector.left);
            board.ApplyMovementAndReset (MoveVector.left);
            InsertAtAnAvailablePosition (board.GetPositionsInColumnMatching (width - 1, default (Color)));
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            board.PushAll (MoveVector.right);
            board.ApplyMovementAndReset (MoveVector.right);
            InsertAtAnAvailablePosition (board.GetPositionsInColumnMatching (0, default (Color)));
        } else if (Input.GetKeyDown (KeyCode.DownArrow)) {
            board.PushAll (MoveVector.down);
            board.ApplyMovementAndReset (MoveVector.down);
            InsertAtAnAvailablePosition (board.GetPositionsInRowMatching (height - 1, default (Color)));
        } else if (Input.GetKeyDown (KeyCode.UpArrow)) {
            board.PushAll (MoveVector.up);
            board.ApplyMovementAndReset (MoveVector.up);
            InsertAtAnAvailablePosition (board.GetPositionsInRowMatching (0, default (Color)));
        }
    }

    void InsertAtAnAvailablePosition (List<IBoardIndex> positions) {
        foreach (IBoardIndex position in positions) {
            int random = Random.Range (1, 4);
            Color color;
            if (random == 1) {
                color = Color.red;
            } else if (random == 2) {
                color = Color.blue;
            } else {
                color = Color.green;
            }
            board.UpdateTile (position, color);
        }
    }
}
