using UnityEngine;

namespace PuzzleBoardFramework {

    public abstract class BoardRenderer<T> : BaseBoard<GameObject>, IUpdatableBoard<T>, IBoardRenderer<T> {
        IUpdatableBoard<T> board;
        Transform parent;

        public BoardRenderer (BaseBoard<T> board, Transform parent) : base (board.width, board.height) {
            this.board = board;
            this.parent = parent;
        }

        public abstract GameObject CreateRenderObject ();
        
        public GameObject GetRenderObject (IBoardIndex position) {
            return GetTile (position);
        }
        
        public void UpdateTile (IBoardIndex position, T value) {
            GameObject obj = GetRenderObject (position);
            if (obj != null) {
                UpdateRenderValue (obj, value);
            }
        }

        public override void MoveTile (IBoardIndex oldPosition, IBoardIndex newPosition) {
            GameObject obj = GetRenderObject (oldPosition);
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
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    DeleteTile (new BoardPosition (x, y));
                }
            }
        }

        T IUpdatableBoard<T>.GetTile (IBoardIndex position) {
            return board.GetTile (position);
        }

        bool IUpdatableBoard<T>.IsPositionValue (IBoardIndex position, T value) {
            return board.IsPositionValue (position, value);
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
            obj.transform.localPosition = new Vector3 (position.X - width/2f + .5f, position.Y - height/2f + .5f, z);
        }

        public virtual void UpdateRenderValue (GameObject obj, T value) {
        }

        public virtual void UpdateRenderRotation (GameObject obj, T value, MoveVector move) {
        } 
    }

}