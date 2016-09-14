using UnityEngine;

namespace PuzzleBoardFramework {

    public abstract class PuzzleBoardRenderer<T> : MonoBehaviour {
        public int width = 4;
        public int height = 4;
        public PuzzleBoard<T> board;

        GameObject[,] renderObjects;
        MergeStrategy<T> mergeStrategy;

        public abstract void UpdateRenderValue (int x, int y, T value);
        public abstract GameObject CreateRenderObject ();
        public abstract MergeStrategy<T> GetMergeStrategy ();

        public bool hasReceivedSecondRecord = false;
        public Record<T> secondRecord;

        public void Start () {
            mergeStrategy = GetMergeStrategy ();
            board = new PuzzleBoard<T> (width, height, mergeStrategy);
            renderObjects = new GameObject[width, height];
            board.RegisterConsumer (OnRecordReceived);
        }

        public virtual void OnRecordReceived (Record<T> record) {
            if (record.type == RecordType.Merge) {
                if (!hasReceivedSecondRecord) {
                    secondRecord = record;
                    hasReceivedSecondRecord = true;
                } else {
                    hasReceivedSecondRecord = false;
                    Record<T> staticRecord = (record.newPosition == record.oldPosition) ? record : secondRecord;
                    Record<T> movingRecord = (staticRecord.Equals (record)) ? secondRecord : record;

                    DestroyRenderObject (staticRecord.newPosition.x, staticRecord.newPosition.y);
                    MoveRenderObject (movingRecord.oldPosition.x, movingRecord.oldPosition.y, movingRecord.newPosition.x, movingRecord.newPosition.y);
                    UpdateRenderValue (movingRecord.newPosition.x, movingRecord.newPosition.y, movingRecord.newValue);
                }
            } else if (record.type == RecordType.Split) {
                if (!hasReceivedSecondRecord) {
                    secondRecord = record;
                    hasReceivedSecondRecord = true;
                } else {
                    hasReceivedSecondRecord = false;
                    Record<T> staticRecord = (record.newPosition == record.oldPosition) ? record : secondRecord;
                    Record<T> movingRecord = (staticRecord.Equals (record)) ? secondRecord : record;

                    MoveRenderObject (movingRecord.oldPosition.x, movingRecord.oldPosition.y, movingRecord.newPosition.x, movingRecord.newPosition.y);
                    UpdateRenderValue (movingRecord.newPosition.x, movingRecord.newPosition.y, movingRecord.newValue);
                    InsertNewRenderObject (staticRecord.newPosition.x, staticRecord.newPosition.y, staticRecord.newValue);
                }
            } else if (record.type == RecordType.Move) {
                // This is a tile that moved into an empty spot.  Find and update it's render cube.
                MoveRenderObject (record.oldPosition.x, record.oldPosition.y, record.newPosition.x, record.newPosition.y);
            } else if (record.type == RecordType.Insert) {
                InsertNewRenderObject (record.newPosition.x, record.newPosition.y, record.newValue);
            } else if (record.type == RecordType.Delete) {
                DestroyRenderObject (record.newPosition.x, record.newPosition.y);
            } else if (record.type == RecordType.Update) {
                UpdateRenderValue (record.newPosition.x, record.newPosition.y, record.newValue);
            }
        }

        public GameObject GetRenderObject (int x, int y) {
            return renderObjects[x,y];
        } 

        public void MoveRenderObject (int oldX, int oldY, int newX, int newY) {
            GameObject obj = renderObjects[oldX, oldY];
            if (obj != null) {
                UpdateRenderPosition (obj, newX, newY);
                renderObjects[newX, newY] = obj;
                renderObjects[oldX,oldY] = null;
            }
        }

        public virtual void UpdateRenderPosition (GameObject obj, int x, int y, int z = 0) {
            obj.transform.localPosition = new Vector3 (x - width/2f + .5f, y - height/2f + .5f, z);
        }

        public void InsertNewRenderObject (int x, int y, T value) {
            GameObject obj = CreateRenderObject ();
            renderObjects[x, y] = obj;
            UpdateRenderPosition (obj, x, y);
            UpdateRenderValue (x, y, value);
            obj.transform.parent = transform;
        }

        public void DestroyRenderObject (int x, int y) {
            if (renderObjects[x,y] != null) {
                Destroy (renderObjects[x,y]);
            }
            renderObjects[x,y] = null;
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
