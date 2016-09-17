using UnityEngine;

namespace PuzzleBoardFramework {

    public abstract class BoardController<T> : MonoBehaviour, IUpdatableBoard<T> {
        public int width = 4;
        public int height = 4;
        public PuzzleBoard<T> board;

        BaseBoard<GameObject> renderObjects;
        MergeStrategy<T> mergeStrategy;

        public abstract void UpdateTile (IBoardIndex position, T value);
        public abstract GameObject CreateRenderObject ();
        public abstract MergeStrategy<T> GetMergeStrategy ();

        public bool hasReceivedSecondRecord = false;
        public Record<T> secondRecord;

        public void Start () {
            mergeStrategy = GetMergeStrategy ();
            board = new PuzzleBoard<T> (width, height, mergeStrategy);
            renderObjects = new BaseBoard<GameObject> (width, height);
            board.RegisterConsumer (OnRecordReceived);
        }

        public virtual void OnRecordReceived (Record<T> record) {
            if (record.type == RecordType.Merge || record.type == RecordType.Split) {
                if (!hasReceivedSecondRecord) {
                    secondRecord = record;
                    hasReceivedSecondRecord = true;
                } else {
                    // Assumes that Merge and Split records always come in pairs
                    hasReceivedSecondRecord = false;
                    Record<T> staticRecord = (record.IsStatic ()) ? record : secondRecord;
                    Record<T> movingRecord = (record.IsStatic ()) ? secondRecord : record;
                    if (record.type == RecordType.Merge) {
                        MergeTile (movingRecord.oldState, movingRecord.newState, movingRecord.newState.Value);
                    } else {
                        SplitTile (movingRecord.oldState, movingRecord.newState, staticRecord.newState.Value, movingRecord.newState.Value);
                    }
                }
            } else if (record.type == RecordType.Move) {
                // This is a tile that moved into an empty spot.  Find and update it's render cube.
                MoveTile (record.oldState, record.newState);
            } else if (record.type == RecordType.Insert) {
                InsertTile (record.newState, record.newState.Value);
            } else if (record.type == RecordType.Delete) {
                DeleteTile (record.newState);
            } else if (record.type == RecordType.Update) {
                UpdateTile (record.newState, record.newState.Value);
            }
        }

        public GameObject GetRenderObject (IBoardIndex position) {
            return renderObjects.GetTile (position);
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
            obj.transform.parent = transform;
        }

        public void DeleteTile (IBoardIndex position) {
            GameObject obj = renderObjects.GetTile (position);
            if (obj != null) {
                Destroy (obj);
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

        public virtual void UpdateRenderPosition (GameObject obj, IBoardIndex position, int z = 0) {
            if (obj == null) {
                return;
            }
            obj.transform.localPosition = new Vector3 (position.X - width/2f + .5f, position.Y - height/2f + .5f, z);
        }
    }

}
