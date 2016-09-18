using UnityEngine;

// TODO - BoardRenderer should grab values directly from the board
namespace PuzzleBoardFramework {

    /// <summary>Provides an IBoardRenderer interface to an existing IBoard instance.</summary>
    public class BoardRenderer<T> : BaseBoard<GameObject>, IBoardRenderer<T> {

        Transform parent;

        public BoardRenderer (IBoard board, Transform parent) : base (board.Width, board.Height) {
            this.parent = parent;
        }
        
        public void UpdateTile (IBoardIndex position, T value) {
            GameObject obj = GetTile (position);
            if (obj != null) {
                UpdateRenderValue (obj, value);
            }
        }

        public override void MoveTile (IBoardIndex oldPosition, IBoardIndex newPosition) {
            GameObject obj = GetTile (oldPosition);
            if (obj != null) {
                UpdateRenderPosition (obj, newPosition);
                base.MoveTile (oldPosition, newPosition);
            }
        }

        public void InsertTile (IBoardIndex position, T value) {
            GameObject obj = CreateRenderObject ();
            base.InsertTile (position, obj);
            UpdateRenderPosition (obj, position);
            UpdateRenderValue (obj, value);
            obj.transform.parent = parent;
        }

        public override void DeleteTile (IBoardIndex position) {
            GameObject obj = GetTile (position);
            if (obj != null) {
                GameObject.Destroy (obj);
                base.DeleteTile (position);
            }
        }

        public void MergeTile (IBoardIndex fromPosition, IBoardIndex toPosition, T value) {
            DeleteTile (toPosition);
            MoveTile (fromPosition, toPosition);
            UpdateTile (toPosition, value);
        }

        public void SplitTile (IBoardIndex fromPosition, IBoardIndex toPosition, T fromValue, T toValue) {
            MoveTile (fromPosition, toPosition);
            UpdateTile (toPosition, toValue);
            InsertTile (fromPosition, fromValue);
        }

        public override void Clear () {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    DeleteTile (new BoardPosition (x, y));
                }
            }
        }

        public virtual GameObject CreateRenderObject () {
            return GameObject.CreatePrimitive (PrimitiveType.Cube);
        }

        public void RotateTile (IBoardIndex position, T value, MoveVector move) {
            GameObject obj = GetTile (position);
            if (obj != null) {
                UpdateRenderRotation (obj, value, move);
            }
        }

        public virtual void UpdateRenderPosition (GameObject obj, IBoardIndex position, int z = 0) {
            if (obj == null) {
                return;
            }
            obj.transform.localPosition = new Vector3 (position.X - Width/2f + .5f, position.Y - Height/2f + .5f, z);
        }

        public virtual void UpdateRenderValue (GameObject obj, T value) {
        }

        public virtual void UpdateRenderRotation (GameObject obj, T value, MoveVector move) {
        } 

    }

}