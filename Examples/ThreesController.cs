using System.Collections.Generic;
using UnityEngine;
using PuzzleBoardFramework;

public class ThreesController : BoardController<int> {

    class ThreesMergeStrategy : IntMergeStrategy {
        public override bool ShouldMerge (int from, int into) {
            return IsEmpty (into) || (from + into == 3) || (from == into && from > 2); 
        }
    }

    class ThreesRenderer : BoardRenderer<int> {
        public ThreesRenderer (BaseBoard<int> board, Transform parent) : base (board, parent) {
        }

        public override GameObject CreateRenderObject () {
            GameObject obj = base.CreateRenderObject ();
            obj.GetComponentInChildren<TextMesh> ().fontSize = 20;
            return obj;
        }
    }


    public override IMergeStrategy<int> GetMergeStrategy () {
        return new ThreesMergeStrategy ();
    }

    public override BoardRenderer<int> GetBoardRenderer (BaseBoard<int> board, Transform parent) {
        return new ThreesRenderer (board, parent);
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
