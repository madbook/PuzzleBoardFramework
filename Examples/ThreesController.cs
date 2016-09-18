using System.Collections.Generic;
using UnityEngine;
using PuzzleBoardFramework;

public class ThreesController : BoardController<int> {

    public override bool ShouldMerge (int from, int into) {
        return BaseBoard<int>.IsEmpty (into) || (from + into == 3) || (from == into && from > 2); 
    }

    public override int GetMergedValue (int from, int into) {
        return from + into;
    }

    public override GameObject CreateRenderObject () {
        GameObject obj = base.CreateRenderObject ();
        obj.GetComponentInChildren<TextMesh> ().fontSize = 20;
        return obj;
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            PushAll (MoveVector.left);
            ApplyMoveVectors (MoveVector.left);
            InsertAtAnAvailablePosition (GetPositionsInColumnMatching (width - 1, 0));
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            PushAll (MoveVector.right);
            ApplyMoveVectors (MoveVector.right);
            InsertAtAnAvailablePosition (GetPositionsInColumnMatching (0, 0));
        } else if (Input.GetKeyDown (KeyCode.DownArrow)) {
            PushAll (MoveVector.down);
            ApplyMoveVectors (MoveVector.down);
            InsertAtAnAvailablePosition (GetPositionsInRowMatching (height - 1, 0));
        } else if (Input.GetKeyDown (KeyCode.UpArrow)) {
            PushAll (MoveVector.up);
            ApplyMoveVectors (MoveVector.up);
            InsertAtAnAvailablePosition (GetPositionsInRowMatching (0, 0));
        }
    }

    void InsertAtAnAvailablePosition (List<IBoardIndex> positions) {
        if (positions.Count == 0) {
            return;
        }
        IBoardIndex position = positions[0];
        int randomValue = Random.Range (1, 4);
        UpdateTile (position, randomValue);
    }

}
