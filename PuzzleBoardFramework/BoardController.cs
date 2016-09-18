using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBoardFramework {

    /// <summary>Combines multiple PuzzleBoardFramework interface implementations into an easy-to-use MonoBehaviour class.</summary>
    public abstract class BoardController<T> : MonoBehaviour,
            IPublisher<Record<T>>,
            IMovableBoard<T>, IUpdateStrategy<T>,
            IBoardSearcher<T>,
            IBoardPusher<T>, IPushStrategy<T>,
            IBoardRenderer<T>, IRenderStrategy<T> {

        public int width = 4;
        public int height = 4;
        
        PublisherBoard<T> board;
        BoardSearcher<T> boardSearcher;
        BoardPusher<T> boardPusher;
        BoardRenderer<T> boardRenderer;

        bool listening = true;
        bool hasReceivedSecondRecord = false;
        Record<T> secondRecord;

        public void Start () {
            board = new PublisherBoard<T> (width, height);
            board.Subscribe (OnRecordReceived);
            boardRenderer = new BoardRenderer<T> (board, transform, this);
            boardSearcher = new BoardSearcher<T> (board);
            boardPusher = new BoardPusher<T> (board, this);
        }

        public virtual void OnRecordReceived (Record<T> record) {
            if (!listening) {
                return;
            }
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

        /*
        IPublisher<T> implemented by board.
        */
        public void Subscribe (Action<Record<T>> subscriber) {
            board.Subscribe (subscriber);
        }

        public void Publish (Record<T> update) {
            board.Publish (update);
        }

        public void UndoRecord (Record<T> record) {
            board.UndoRecord (record);
        }

        /*
        IMovableBoard<T> implemented by board.
        */
        public int Width {
            get { return board.Width; }
        }

        public int Height {
            get { return board.Width; }
        }

        public bool IsValidIndex2D (IBoardIndex index) {
            return board.IsValidIndex2D (index);
        }

        public T GetTile (IBoardIndex position) {
            return board.GetTile (position);
        }

        public bool IsPositionValue (IBoardIndex position, T value) {
            return board.IsPositionValue (position, value);
        }

        public void UpdateTile (IBoardIndex position, T value) {
            board.UpdateTile (position, value);
        }

        public void UpdateTiles (List<IBoardIndex> positions, T value) {
            board.UpdateTiles (positions, value);
        }

        public void DeleteTile (IBoardIndex position) {
            board.DeleteTile (position);
        }

        public void InsertTile (IBoardIndex position, T value) {
            board.InsertTile (position, value);
        }

        public void Clear () {
            listening = false;
            board.Clear ();
            boardRenderer.Clear ();
            listening = true;
        }

        public void MoveTile (IBoardIndex fromPosition, IBoardIndex toPosition) {
            board.MoveTile (fromPosition, toPosition);
        }

        public void MergeTile (IBoardIndex fromPosition, IBoardIndex toPosition, T value) {
            board.MergeTile (fromPosition, toPosition, value);
        }

        public void SplitTile (IBoardIndex fromPosition, IBoardIndex toPosition, T fromValue, T toValue) {
            board.SplitTile (fromPosition, toPosition, fromValue, toValue);
        }

        /*
        IUpdateStrategy<T> implemented by board.
        */
        public virtual bool ShouldUpdate (IBoardIndex position, T value) {
            return board.ShouldUpdate (position, value);
        }

        public virtual bool ShouldInsert (IBoardIndex position, T value) {
            return board.ShouldInsert (position, value);
        }

        public virtual bool ShouldDelete (IBoardIndex position) {
            return board.ShouldDelete (position);
        }

        /*
        IBoardPusher<T> implemented by boardPusher.
        */
        public void PushAll (MoveVector push) {
            boardPusher.PushAll (push);
        }

        public void PushTile (IBoardIndex position, MoveVector push) {
            boardPusher.PushTile (position, push);
        }
 
        public void PushTiles (List<IBoardIndex> positions, MoveVector push) {
            boardPusher.PushTiles (positions, push);
        }

        public void ApplyMoveVectors () {
            boardPusher.ApplyMoveVectors ();
        }

        public void ApplyMoveVectors (MoveVector move) {
            boardPusher.ApplyMoveVectors (move);
        }

        public void PushAllMatching (MoveVector move, T matchValue) {
            boardPusher.PushAllMatching (move, matchValue);
        }

        /*
        IPushStrategy<T> implemented by boardPusher.
        */
        public virtual bool ShouldMove (IBoardIndex from, IBoardIndex into) {
            return boardPusher.ShouldMove (from, into);
        }

        /// <summary>Determines if the from value should merge with the into value.</summary>
        public virtual bool ShouldMerge (IBoardIndex from, IBoardIndex into) {
            return boardPusher.ShouldMerge (from, into);
        }

        /// <summary>Provides a new value when the two given values merge.</summary>
        public virtual T GetMergedValue (IBoardIndex from, IBoardIndex into) {
            return boardPusher.GetMergedValue (from, into);
        }

        public virtual bool ShouldPush (IBoardIndex from, IBoardIndex into) {
            return boardPusher.ShouldPush (from, into);
        }

        /*
        IBoardSearcher<T> implemented by boardSearcher.
        */
        public List<IBoardIndex> GetPositionsMatching (T matchValue) {
            return boardSearcher.GetPositionsMatching (matchValue);
        }

        public List<IBoardIndex> GetPositionsMatching (params T[] valuesToMatch) {
            return boardSearcher.GetPositionsMatching (valuesToMatch);
        }

        public List<IBoardIndex> GetPositionsInRow (int row) {
            return boardSearcher.GetPositionsInRow (row);
        }

        public List<IBoardIndex> GetPositionsInColumn (int col) {
            return boardSearcher.GetPositionsInColumn (col);
        }

        public List<IBoardIndex> GetPositionsInRowMatching (int row, T matchValue) {
            return boardSearcher.GetPositionsInRowMatching (row, matchValue);
        }

        public List<IBoardIndex> GetPositionsInColumnMatching (int col, T matchValue) {
            return boardSearcher.GetPositionsInColumnMatching (col, matchValue);
        }

        public List<IBoardIndex> GetIdenticalAdjacentPositions (T value, IBoardIndex position) {
            return boardSearcher.GetIdenticalAdjacentPositions (value, position);
        }

        /*
        IBoardRenderer<T> implemented by boardRenderer.
        */
        public virtual void RotateTile (IBoardIndex position, T value, MoveVector move) {
            boardRenderer.RotateTile (position, value, move);
        }

        /*
        IRenderStrategy<T> implemented by boardRenderer.
        */
        public virtual GameObject CreateRenderObject () {
            return boardRenderer.CreateRenderObject ();
        }

        public virtual void UpdateRenderPosition (GameObject obj, IBoardIndex position, int z = 0) {
            boardRenderer.UpdateRenderPosition (obj, position, z);
        }

        public virtual void UpdateRenderValue (GameObject obj, T value) {
            boardRenderer.UpdateRenderValue (obj, value);
        }

        public virtual void UpdateRenderRotation (GameObject obj, T value, MoveVector move) {
            boardRenderer.UpdateRenderRotation (obj, value, move);
        }

    }

}
