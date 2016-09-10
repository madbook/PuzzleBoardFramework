using System.Collections.Generic;
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

public class ColorGameRenderer : PuzzleBoardRenderer<Color> {
    public override MergeStrategy<Color> GetMergeStrategy () {
        return new ColorMergeStrategy ();
    }

    public override void UpdateRenderValue (int x, int y, Color value) {
        GameObject obj = GetRenderObject (x, y);
        obj.GetComponent<MeshRenderer> ().material.color = value;
    }

    public override GameObject CreateRenderObject () {
        GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
        return obj;
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            board.PushAll (MoveVector.left);
            board.ApplyMovementAndReset (MoveVector.left);
            InsertAtAnAvailablePosition (board.GetPositionsInColumnMatching (default (Color), width - 1));
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            board.PushAll (MoveVector.right);
            board.ApplyMovementAndReset (MoveVector.right);
            InsertAtAnAvailablePosition (board.GetPositionsInColumnMatching (default (Color), 0));
        } else if (Input.GetKeyDown (KeyCode.DownArrow)) {
            board.PushAll (MoveVector.down);
            board.ApplyMovementAndReset (MoveVector.down);
            InsertAtAnAvailablePosition (board.GetPositionsInRowMatching (default (Color), height - 1));
        } else if (Input.GetKeyDown (KeyCode.UpArrow)) {
            board.PushAll (MoveVector.up);
            board.ApplyMovementAndReset (MoveVector.up);
            InsertAtAnAvailablePosition (board.GetPositionsInRowMatching (default (Color), 0));
        }
    }

    void InsertAtAnAvailablePosition (List<Index2D> positions) {
        foreach (Index2D position in positions) {
            int random = Random.Range (1, 4);
            Color color;
            if (random == 1) {
                color = Color.red;
            } else if (random == 2) {
                color = Color.blue;
            } else {
                color = Color.green;
            }
            board.UpdateTile (position.x, position.y, color);
        }
    }
}
