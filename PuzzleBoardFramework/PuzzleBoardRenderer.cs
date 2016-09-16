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
                    Record<T> staticRecord = (record.newPosition.Equals (record.oldPosition)) ? record : secondRecord;
                    Record<T> movingRecord = (staticRecord.Equals (record)) ? secondRecord : record;
                    DestroyRenderObject (staticRecord.newPosition.X, staticRecord.newPosition.Y);
                    MoveRenderObject (movingRecord.oldPosition.X, movingRecord.oldPosition.Y, movingRecord.newPosition.X, movingRecord.newPosition.Y);
                    UpdateRenderValue (movingRecord.newPosition.X, movingRecord.newPosition.Y, movingRecord.newValue);
                }
            } else if (record.type == RecordType.Split) {
                if (!hasReceivedSecondRecord) {
                    secondRecord = record;
                    hasReceivedSecondRecord = true;
                } else {
                    hasReceivedSecondRecord = false;
                    Record<T> staticRecord = (record.newPosition.Equals (record.oldPosition)) ? record : secondRecord;
                    Record<T> movingRecord = (staticRecord.Equals (record)) ? secondRecord : record;

                    MoveRenderObject (movingRecord.oldPosition.X, movingRecord.oldPosition.Y, movingRecord.newPosition.X, movingRecord.newPosition.Y);
                    UpdateRenderValue (movingRecord.newPosition.X, movingRecord.newPosition.Y, movingRecord.newValue);
                    InsertNewRenderObject (staticRecord.newPosition.X, staticRecord.newPosition.Y, staticRecord.newValue);
                }
            } else if (record.type == RecordType.Move) {
                // This is a tile that moved into an empty spot.  Find and update it's render cube.
                MoveRenderObject (record.oldPosition.X, record.oldPosition.Y, record.newPosition.X, record.newPosition.Y);
            } else if (record.type == RecordType.Insert) {
                InsertNewRenderObject (record.newPosition.X, record.newPosition.Y, record.newValue);
            } else if (record.type == RecordType.Delete) {
                DestroyRenderObject (record.newPosition.X, record.newPosition.Y);
            } else if (record.type == RecordType.Update) {
                UpdateRenderValue (record.newPosition.X, record.newPosition.Y, record.newValue);
            }
        }

        public GameObject GetRenderObject (int x, int y) {
            return renderObjects.GetTile (x, y);
        } 

        public void MoveRenderObject (int oldX, int oldY, int newX, int newY) {
            GameObject obj = GetRenderObject (oldX, oldY);
            if (obj != null) {
                UpdateRenderPosition (obj, newX, newY);
                renderObjects.UpdateTile (newX, newY, obj);
                renderObjects.UpdateTile (oldX, oldY, null);
            }
        }

        public virtual void UpdateRenderPosition (GameObject obj, int x, int y, int z = 0) {
            obj.transform.localPosition = new Vector3 (x - width/2f + .5f, y - height/2f + .5f, z);
        }

        public void InsertNewRenderObject (int x, int y, T value) {
            GameObject obj = CreateRenderObject ();
            renderObjects.UpdateTile (x, y, obj);
            UpdateRenderPosition (obj, x, y);
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
