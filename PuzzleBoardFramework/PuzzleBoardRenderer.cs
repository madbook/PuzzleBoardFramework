using UnityEngine;

namespace PuzzleBoardFramework {

    public abstract class PuzzleBoardRenderer<T> : MonoBehaviour {
        public int width = 4;
        public int height = 4;
        public PuzzleBoard<T> board;

        BaseBoard<GameObject> renderObjects;
        MergeStrategy<T> mergeStrategy;

        public abstract void UpdateRenderValue (IBoardIndex position, T value);
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
            if (record.type == RecordType.Merge) {
                if (!hasReceivedSecondRecord) {
                    secondRecord = record;
                    hasReceivedSecondRecord = true;
                } else {
                    hasReceivedSecondRecord = false;
                    Record<T> staticRecord = (record.IsStatic ()) ? record : secondRecord;
                    Record<T> movingRecord = (record.IsStatic ()) ? secondRecord : record;
                    DestroyRenderObject (staticRecord.newState);
                    MoveRenderObject (movingRecord.oldState, movingRecord.newState);
                    UpdateRenderValue (movingRecord.newState, movingRecord.newState.Value);
                }
            } else if (record.type == RecordType.Split) {
                if (!hasReceivedSecondRecord) {
                    secondRecord = record;
                    hasReceivedSecondRecord = true;
                } else {
                    hasReceivedSecondRecord = false;
                    Record<T> staticRecord = (record.IsStatic ()) ? record : secondRecord;
                    Record<T> movingRecord = (record.IsStatic ()) ? secondRecord : record;
                    MoveRenderObject (movingRecord.oldState, movingRecord.newState);
                    UpdateRenderValue (movingRecord.newState, movingRecord.newState.Value);
                    InsertNewRenderObject (staticRecord.newState, staticRecord.newState.Value);
                }
            } else if (record.type == RecordType.Move) {
                // This is a tile that moved into an empty spot.  Find and update it's render cube.
                MoveRenderObject (record.oldState, record.newState);
            } else if (record.type == RecordType.Insert) {
                InsertNewRenderObject (record.newState, record.newState.Value);
            } else if (record.type == RecordType.Delete) {
                DestroyRenderObject (record.newState);
            } else if (record.type == RecordType.Update) {
                UpdateRenderValue (record.newState, record.newState.Value);
            }
        }

        public GameObject GetRenderObject (IBoardIndex position) {
            return renderObjects.GetTile (position);
        }

        public void MoveRenderObject (IBoardIndex oldPosition, IBoardIndex newPosition) {
            GameObject obj = GetRenderObject (oldPosition);
            if (obj != null) {
                UpdateRenderPosition (obj, newPosition);
                renderObjects.MoveTile (oldPosition, newPosition);
            }
        }

        public virtual void UpdateRenderPosition (GameObject obj, IBoardIndex position, int z = 0) {
            if (obj == null) {
                return;
            }
            obj.transform.localPosition = new Vector3 (position.X - width/2f + .5f, position.Y - height/2f + .5f, z);
        }

        public void InsertNewRenderObject (IBoardIndex position, T value) {
            GameObject obj = CreateRenderObject ();
            renderObjects.UpdateTile (position, obj);
            UpdateRenderPosition (obj, position);
            UpdateRenderValue (position, value);
            obj.transform.parent = transform;
        }

        public void DestroyRenderObject (IBoardIndex position) {
            GameObject obj = renderObjects.GetTile (position);
            if (obj != null) {
                Destroy (obj);
                renderObjects.UpdateTile (position, null);
            }
        }

        public void ClearRenderObjects () {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    DestroyRenderObject (new BoardPosition (x, y));
                }
            }
        }
    }

}
