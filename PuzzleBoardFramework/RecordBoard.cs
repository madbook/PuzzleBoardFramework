using System;

namespace PuzzleBoardFramework {
    public class RecordBoard<T> : BaseBoard<T> {
        Publisher<Record<T>> publisher = new Publisher<Record<T>> ();

        public RecordBoard (int width, int height) : base (width, height) {
        }

        /// <summary>Register a callback method to be called when a Record is added via AddRecord.</summary>
        public void RegisterConsumer (Action<Record<T>> callback) {
            // TODO move this out into the controller
            publisher.Subscribe (callback);
        }

        /// <summary>Insert, update, or delete the value at the given Index2D position.</summary>
        public override void UpdateTiles (IBoardIndex position, T value) {
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
                new BoardState<T> (position.X, position.Y, oldValue),
                new BoardState<T> (position.X, position.Y, value)
            ));

            SetTile (position, value);
        }

        public override void MoveTile (IBoardIndex fromPosition, IBoardIndex toPosition) {
            if (!GetTile (toPosition).Equals (default (T))) {
                return;
            }

            T value = GetTile (fromPosition);
            SetTile (toPosition, value);
            SetTile (fromPosition, default (T));
            AddRecord (new Record<T> (
                RecordType.Move,
                new BoardState<T> (fromPosition.X, fromPosition.Y, value),
                new BoardState<T> (toPosition.X, toPosition.Y, value)
            ));
        }

        public override void MergeTile (IBoardIndex fromPosition, IBoardIndex toPosition, T value) {
            T valueFrom = GetTile (fromPosition);
            T valueInto = GetTile (toPosition);
            SetTile (toPosition, value);
            SetTile (fromPosition, default (T));

            AddRecord (new Record<T> (
                RecordType.Merge,
                new BoardState<T> (fromPosition.X, fromPosition.Y, valueFrom),
                new BoardState<T> (toPosition.X, toPosition.Y, value)
            ));

            AddRecord (new Record<T> (
                RecordType.Merge,
                new BoardState<T> (toPosition.X, toPosition.Y, valueInto),
                new BoardState<T> (toPosition.X, toPosition.Y, value)
            ));
        }

        public void UndoRecord (Record<T> record) {
            if (record.type == RecordType.Move) {
                if (!IsPositionValue (record.oldState, default (T))) {
                    return;
                }
                SetTile (record.newState, default (T));
            }

            SetTile (record.oldState, record.oldState.Value);
            AddRecord (new Record<T> (
                Record.GetOppositeRecordType (record.type),
                record.newState,
                record.oldState
            ));
        }

        /// <summary>Broadcasts a Record to any consumers added with RegisterConsumer</summary>
        void AddRecord (Record<T> record) {
            publisher.Publish (record);
        }
    }
}
