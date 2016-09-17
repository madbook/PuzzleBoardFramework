using System.Collections.Generic;
using UnityEngine;
using PuzzleBoardFramework;

public class ThreesMergeStrategy : IntMergeStrategy {
    public override bool ShouldMerge (int from, int into) {
        return IsEmpty (into) || (from + into == 3) || (from == into && from > 2); 
    }
}

public class ThreesRenderer : BoardController<int> {
    public override MergeStrategy<int> GetMergeStrategy () {
        return new ThreesMergeStrategy ();
    }

    public override void UpdateTile (IBoardIndex position, int value) {
        GameObject obj = GetRenderObject (position);
        obj.GetComponentInChildren<TextMesh> ().text = value.ToString ();
    }

    public override GameObject CreateRenderObject () {
        GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
        GameObject text = new GameObject ("Text");
        TextMesh textMesh = text.AddComponent<TextMesh> (); 
        textMesh.fontSize = 20;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.black;
        text.transform.parent = obj.transform;
        text.transform.localScale = new Vector3 (.3f, .3f, .3f);
        text.transform.localPosition = Vector3.zero;
        return obj;
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            board.PushAll (MoveVector.left);
            board.ApplyMovementAndReset (MoveVector.left);
            InsertAtAnAvailablePosition (board.GetPositionsInColumnMatching (0, width - 1));
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            board.PushAll (MoveVector.right);
            board.ApplyMovementAndReset (MoveVector.right);
            InsertAtAnAvailablePosition (board.GetPositionsInColumnMatching (0, 0));
        } else if (Input.GetKeyDown (KeyCode.DownArrow)) {
            board.PushAll (MoveVector.down);
            board.ApplyMovementAndReset (MoveVector.down);
            InsertAtAnAvailablePosition (board.GetPositionsInRowMatching (0, height - 1));
        } else if (Input.GetKeyDown (KeyCode.UpArrow)) {
            board.PushAll (MoveVector.up);
            board.ApplyMovementAndReset (MoveVector.up);
            InsertAtAnAvailablePosition (board.GetPositionsInRowMatching (0, 0));
        }
    }

    void InsertAtAnAvailablePosition (List<IBoardIndex> positions) {
        if (positions.Count == 0) {
            return;
        }
        IBoardIndex position = positions[0];
        board.UpdateTile (position, Random.Range (1, 4));
    }
}
