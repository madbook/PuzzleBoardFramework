using System.Collections.Generic;
using UnityEngine;
using PuzzleBoardFramework;

public class ColorGameController : BoardController<Color> {

    class ColorMergeStrategy : GenericMergeStrategy<Color> {
        public override bool ShouldMerge (Color from, Color into) {
            return true;
        }

        public override Color Merge (Color from, Color into) {
            return Color.Lerp (from, into, .5f);
        }
    }


    public override IMergeStrategy<Color> GetMergeStrategy () {
        return new ColorMergeStrategy ();
    }

    public override GameObject CreateRenderObject () {
        return GameObject.CreatePrimitive (PrimitiveType.Cube);
    }

    public override void UpdateRenderValue (GameObject obj, Color value) {
        obj.GetComponent<MeshRenderer> ().material.color = value;
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            PushAll (MoveVector.left);
            ApplyMoveVectors (MoveVector.left);
            InsertAtAnAvailablePosition (GetPositionsInColumnMatching (width - 1, default (Color)));
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            PushAll (MoveVector.right);
            ApplyMoveVectors (MoveVector.right);
            InsertAtAnAvailablePosition (GetPositionsInColumnMatching (0, default (Color)));
        } else if (Input.GetKeyDown (KeyCode.DownArrow)) {
            PushAll (MoveVector.down);
            ApplyMoveVectors (MoveVector.down);
            InsertAtAnAvailablePosition (GetPositionsInRowMatching (height - 1, default (Color)));
        } else if (Input.GetKeyDown (KeyCode.UpArrow)) {
            PushAll (MoveVector.up);
            ApplyMoveVectors (MoveVector.up);
            InsertAtAnAvailablePosition (GetPositionsInRowMatching (0, default (Color)));
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
            UpdateTile (position, color);
        }
    }

}
