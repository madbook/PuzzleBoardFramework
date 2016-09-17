using UnityEngine;

namespace PuzzleBoardFramework {

    public abstract class BoardController<T> : MonoBehaviour {
        public int width = 4;
        public int height = 4;
        
        protected BoardRenderer<T> boardRenderer;
        protected PuzzleBoard<T> board;
        IMergeStrategy<T> mergeStrategy;

        bool hasReceivedSecondRecord = false;
        Record<T> secondRecord;

        public abstract IMergeStrategy<T> GetMergeStrategy ();
        public abstract BoardRenderer<T> GetBoardRenderer (BaseBoard<T> board, Transform parent);

        public void Start () {
            mergeStrategy = GetMergeStrategy ();
            board = new PuzzleBoard<T> (width, height, mergeStrategy);
            boardRenderer = GetBoardRenderer (board, transform);
            board.Subscribe (OnRecordReceived);
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
                        boardRenderer.MergeTile (movingRecord.oldState, movingRecord.newState, movingRecord.newState.Value);
                    } else {
                        boardRenderer.SplitTile (movingRecord.oldState, movingRecord.newState, staticRecord.newState.Value, movingRecord.newState.Value);
                    }
                }
            } else if (record.type == RecordType.Move) {
                // This is a tile that moved into an empty spot.  Find and update it's render cube.
                boardRenderer.MoveTile (record.oldState, record.newState);
            } else if (record.type == RecordType.Insert) {
                boardRenderer.InsertTile (record.newState, record.newState.Value);
            } else if (record.type == RecordType.Delete) {
                boardRenderer.DeleteTile (record.newState);
            } else if (record.type == RecordType.Update) {
                boardRenderer.UpdateTile (record.newState, record.newState.Value);
            }
        }
    }

}
