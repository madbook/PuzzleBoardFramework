using System.Collections.Generic;
using UnityEngine;
using PuzzleBoardFramework;

public class ThreesMergeStrategy : IntMergeStrategy {
    public override bool ShouldMerge (int from, int into) {
        Debug.Log ("Should merge? " + from + " => " + into);
        return IsEmpty (into) || (from + into == 3) || (from == into && from > 2); 
    }
}

public class ThreesRenderer : BoardRenderer<int> {
    public ThreesRenderer (BaseBoard<int> board, Transform parent) : base (board, parent) {
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

    public override void UpdateRenderValue (GameObject obj, int value) {
        obj.GetComponentInChildren<TextMesh> ().text = value.ToString ();
    }
}

public class ThreesController : BoardController<int> {
    public override MergeStrategy<int> GetMergeStrategy () {
        return new ThreesMergeStrategy ();
    }

    public override BoardRenderer<int> GetBoardRenderer (BaseBoard<int> board, Transform parent) {
        return new ThreesRenderer (board, parent);
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            board.PushAll (MoveVector.left);
            board.ApplyMovementAndReset (MoveVector.left);
            InsertAtAnAvailablePosition (board.GetPositionsInColumnMatching (width - 1, 0));
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            board.PushAll (MoveVector.right);
            board.ApplyMovementAndReset (MoveVector.right);
            InsertAtAnAvailablePosition (board.GetPositionsInColumnMatching (0, 0));
        } else if (Input.GetKeyDown (KeyCode.DownArrow)) {
            board.PushAll (MoveVector.down);
            board.ApplyMovementAndReset (MoveVector.down);
            InsertAtAnAvailablePosition (board.GetPositionsInRowMatching (height - 1, 0));
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
        int randomValue = Random.Range (1, 4);
        board.UpdateTile (position, randomValue);
    }
}
