using System;
using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class PuzzleBoard<T> : BaseBoard<T>, IPushableBoard {
        PushBoard<T> pushBoard;
        Publisher<Record<T>> publisher = new Publisher<Record<T>> ();

        /// <summary>Create a new PuzzleBoard using a default MergeStrategy.</summary>
        public PuzzleBoard (int width, int height) : base (width, height) {
            MergeStrategy<T> mergeStrategy = MergeStrategy.GetDefaultStrategy<T> ();
            pushBoard = new PushBoard<T> (width, height, this, mergeStrategy);
        }

        /// <summary>Create a new PuzzleBoard with a custom MergeStrategy.</summary>
        public PuzzleBoard (int width, int height, MergeStrategy<T> mergeStrategy) : base (width, height) {
            pushBoard = new PushBoard<T> (width, height, this, mergeStrategy);
        }

        /// <summary>Register a callback method to be called when a Record is added via AddRecord.</summary>
        public void RegisterConsumer (Action<Record<T>> callback) {
            // TODO move this out into the controller
            publisher.Subscribe (callback);
        }

        /// <summary>Try to move each cell by it's currently set move vector, then reset all move vectors.</summary>
        public void ApplyMovementAndReset () {
            pushBoard.ApplyMoveVectors ();
        }

        /// <summary>Try to move each cell with the given MoveVector set, then reset all move vectors.</summary>
        public void ApplyMovementAndReset (MoveVector push) {
            pushBoard.ApplyMoveVectors (push);
        }

        /// <summary>Set movement vectors on cells to the given direction.</summary>
        public void PushAll (MoveVector push) {
            pushBoard.PushAll (push);
        }

        /// <summary>Set the movement vector of the cell at the given x and y coordinates to the given direction.</summary>
        public void PushTile (int x, int y, MoveVector push) {
            pushBoard.PushTile (x, y, push);
        }

        /// <summary>Set the movement vector of the cell at the given Index2D position to the given direction.</summary>
        public void PushTile (IBoardIndex position, MoveVector push) {
            pushBoard.PushTile (position, push);
        }

        /// <summary>Set the movement vector of each cell in a list of Index2D positions to the given direction.</summary>
        public void PushTile (List<IBoardIndex> positions, MoveVector push) {
            pushBoard.PushTile (positions, push);
        }

        /// <summary>Set movement vectors at all cells matching the value to the given direction.</summary>
        public void PushAllMatching (MoveVector push, T matchValue)  {
            pushBoard.PushAllMatching (push, matchValue);
        }

        /// <summary>Insert, update, or delete the value at the given Index2D position.</summary>
        public override void UpdateTile (IBoardIndex position, T value) {
            T oldValue = GetTile (position);

            bool newIsEmpty = value.Equals (default (T));
            bool oldIsEmpty = oldValue.Equals (default (T));

            if (newIsEmpty && oldIsEmpty) {
                return;
            }

            RecordType type;
            
            if (oldIsEmpty) {
                type = RecordType.Insert;
            } else if (newIsEmpty) {
                type = RecordType.Delete;
            } else {
                type = RecordType.Update;
            }
            
            AddRecord (new Record<T> (
                type,
                new BoardPosition (position.X, position.Y),
                new BoardPosition (position.X, position.Y),
                oldValue,
                value
            ));

            SetTile (position, value);
        }

        public void MoveTile (IBoardIndex fromPosition, IBoardIndex toPosition) {
            if (!GetTile (toPosition).Equals (default (T))) {
                return;
            }

            T value = GetTile (fromPosition);
            SetTile (toPosition, value);
            SetTile (fromPosition, default (T));
            AddRecord (new Record<T> (
                RecordType.Move,
                new BoardPosition (fromPosition.X, fromPosition.Y),
                new BoardPosition (toPosition.X, toPosition.Y),
                value,
                value
            ));
        }

        public void MergeTile (IBoardIndex fromPosition, IBoardIndex toPosition, T value) {
            T valueFrom = GetTile (fromPosition);
            T valueInto = GetTile (toPosition);
            SetTile (toPosition, value);
            SetTile (fromPosition, default (T));

            AddRecord (new Record<T> (
                RecordType.Merge,
                new BoardPosition (fromPosition.X, fromPosition.Y),
                new BoardPosition (toPosition.X, toPosition.Y),
                valueFrom,
                value
            ));

            AddRecord (new Record<T> (
                RecordType.Merge,
                new BoardPosition (toPosition.X, toPosition.Y),
                new BoardPosition (toPosition.X, toPosition.Y),
                valueInto,
                value
            ));
        }

        public void UndoRecord (Record<T> record) {
            if (record.type == RecordType.Move) {
                if (!IsPositionValue (record.oldPosition, default (T))) {
                    return;
                }
                SetTile (record.newPosition, default (T));
            }

            SetTile (record.oldPosition, record.oldValue);
            AddRecord (new Record<T> (
                Record.GetOppositeRecordType (record.type),
                new BoardPosition (record.newPosition.X, record.newPosition.Y),
                new BoardPosition (record.oldPosition.X, record.oldPosition.Y),
                record.newValue,
                record.oldValue
            ));
        }

        /// <summary>Resets all MoveVectors.  Sets all values to default, using the current mergeStrategy.</summary>
        public override void Clear () {
            pushBoard.Clear ();
            base.Clear ();
        }

        /// <summary>Broadcasts a Record to any consumers added with RegisterConsumer</summary>
        void AddRecord (Record<T> record) {
            publisher.Publish (record);
        }
    }

}
