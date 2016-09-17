using UnityEngine;

namespace PuzzleBoardFramework {

    public interface IAnimatableRenderer {
        bool Animating { get; set; }
    }

    public abstract class BoardRenderer<T> : IUpdatableBoard<T> {
        protected int width;
        protected int height;
        Transform parent;
        BaseBoard<GameObject> renderObjects;

        public BoardRenderer (BaseBoard<T> board, Transform parent) {
            this.width = board.width;
            this.height = board.height;
            this.parent = parent;
            renderObjects = new BaseBoard<GameObject> (width, height);
        }

        public abstract GameObject CreateRenderObject ();
        
        public GameObject GetRenderObject (IBoardIndex position) {
            return renderObjects.GetTile (position);
        }
        
        public void UpdateTile (IBoardIndex position, T value) {
            GameObject obj = GetRenderObject (position);
            if (obj != null) {
                UpdateRenderValue (obj, value);
            }
        }

        public void MoveTile (IBoardIndex oldPosition, IBoardIndex newPosition) {
            GameObject obj = GetRenderObject (oldPosition);
            if (obj != null) {
                UpdateRenderPosition (obj, newPosition);
                renderObjects.MoveTile (oldPosition, newPosition);
            }
        }

        public void InsertTile (IBoardIndex position, T value) {
            GameObject obj = CreateRenderObject ();
            renderObjects.UpdateTile (position, obj);
            UpdateRenderPosition (obj, position);
            UpdateTile (position, value);
            obj.transform.parent = parent;
        }

        public void DeleteTile (IBoardIndex position) {
            GameObject obj = renderObjects.GetTile (position);
            if (obj != null) {
                GameObject.Destroy (obj);
                renderObjects.UpdateTile (position, null);
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

        public void Clear () {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    DeleteTile (new BoardPosition (x, y));
                }
            }
        }

        public void RotateTile (IBoardIndex position, T value, MoveVector move) {
            GameObject obj = renderObjects.GetTile (position);
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