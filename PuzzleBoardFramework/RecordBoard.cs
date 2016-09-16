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

        public override void MoveTile (IBoardIndex fromPosition, IBoardIndex toPosition) {
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

        public override void MergeTile (IBoardIndex fromPosition, IBoardIndex toPosition, T value) {
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

        /// <summary>Broadcasts a Record to any consumers added with RegisterConsumer</summary>
        void AddRecord (Record<T> record) {
            publisher.Publish (record);
        }
    }
}
