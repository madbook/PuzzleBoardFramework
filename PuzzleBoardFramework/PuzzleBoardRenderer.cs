using UnityEngine;

namespace PuzzleBoardFramework {

    public abstract class PuzzleBoardRenderer<T> : MonoBehaviour {
        public int width = 4;
        public int height = 4;
        public PuzzleBoard<T> board;

        BaseBoard<GameObject> renderObjects;
        MergeStrategy<T> mergeStrategy;

        public abstract void UpdateRenderValue (int x, int y, T value);
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
                    DestroyRenderObject (staticRecord.newState.X, staticRecord.newState.Y);
                    MoveRenderObject (movingRecord.oldState.X, movingRecord.oldState.Y, movingRecord.newState.X, movingRecord.newState.Y);
                    UpdateRenderValue (movingRecord.newState.X, movingRecord.newState.Y, movingRecord.newState.Value);
                    Record<T> movingRecord = (record.IsStatic ()) ? secondRecord : record;
                }
            } else if (record.type == RecordType.Split) {
                if (!hasReceivedSecondRecord) {
                    secondRecord = record;
                    hasReceivedSecondRecord = true;
                } else {
                    hasReceivedSecondRecord = false;

                    MoveRenderObject (movingRecord.oldState.X, movingRecord.oldState.Y, movingRecord.newState.X, movingRecord.newState.Y);
                    UpdateRenderValue (movingRecord.newState.X, movingRecord.newState.Y, movingRecord.newState.Value);
                    InsertNewRenderObject (staticRecord.newState.X, staticRecord.newState.Y, staticRecord.newState.Value);
                    Record<T> staticRecord = (record.IsStatic ()) ? record : secondRecord;
                    Record<T> movingRecord = (record.IsStatic ()) ? secondRecord : record;
                }
            } else if (record.type == RecordType.Move) {
                // This is a tile that moved into an empty spot.  Find and update it's render cube.
                MoveRenderObject (record.oldState.X, record.oldState.Y, record.newState.X, record.newState.Y);
            } else if (record.type == RecordType.Insert) {
                InsertNewRenderObject (record.newState.X, record.newState.Y, record.newState.Value);
            } else if (record.type == RecordType.Delete) {
                DestroyRenderObject (record.newState.X, record.newState.Y);
            } else if (record.type == RecordType.Update) {
                UpdateRenderValue (record.newState.X, record.newState.Y, record.newState.Value);
            }
        }

        public GameObject GetRenderObject (int x, int y) {
            return renderObjects.GetTile (x, y);
        }

        public GameObject GetRenderObject (IBoardIndex position) {
            return renderObjects.GetTile (position);
        }

        public void MoveRenderObject (int oldX, int oldY, int newX, int newY) {
            MoveRenderObject (new BoardPosition (oldX, oldY), new BoardPosition (newX, newY));
        }

        public void MoveRenderObject (IBoardIndex oldPosisition, IBoardIndex newPosition) {
            GameObject obj = GetRenderObject (oldPosisition);
            if (obj != null) {
                UpdateRenderPosition (obj, newPosition);
                renderObjects.UpdateTile (newPosition, obj);
                renderObjects.UpdateTile (oldPosisition, null);
            }
        }

        public virtual void UpdateRenderPosition (GameObject obj, IBoardIndex position, int z = 0) {
            obj.transform.localPosition = new Vector3 (position.X - width/2f + .5f, position.Y - height/2f + .5f, z);
        }

        public void InsertNewRenderObject (int x, int y, T value) {
            GameObject obj = CreateRenderObject ();
            renderObjects.UpdateTile (x, y, obj);
            UpdateRenderPosition (obj, new BoardPosition (x, y));
            UpdateRenderValue (x, y, value);
            obj.transform.parent = transform;
        }

        public void DestroyRenderObject (int x, int y) {
            GameObject obj = renderObjects.GetTile (x, y);
            if (obj != null) {
                Destroy (obj);
                renderObjects.UpdateTile (x, y, null);
            }
        }

        public void ClearRenderObjects () {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    DestroyRenderObject (x, y);
                }
            }
        }
    }

}
