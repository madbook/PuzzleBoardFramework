using System;

namespace PuzzleBoardFramework {

    /// <summary>Extends the BaseBoard implementation with the IPublisher interface.</summary>
    /// <remarks>
    ///     Use the Subscribe method to receive records of updates made to the board's state.
    /// </remarks>
    public class PublisherBoard<T> : BaseBoard<T>, IUpdatableBoard<T>, IPublisher<Record<T>> {

        Publisher<Record<T>> publisher = new Publisher<Record<T>> ();

        public PublisherBoard (int width, int height) : base (width, height) {
        }

        /// <summary>Register a callback method to be called when a Record is added via AddRecord.</summary>
        public void Subscribe (Action<Record<T>> callback) {
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
            } else if (oldIsEmpty) {
                InsertTile (position, value);
            } else if (newIsEmpty) {
                DeleteTile (position);
            } else {
                // TODO - either make the base behavior defer to the other methods, or
                // require callers to call the correct method
                base.UpdateTile (position, value);
                Publish (new Record<T> (
                    RecordType.Update,
                    new BoardState<T> (position.X, position.Y, oldValue),
                    new BoardState<T> (position.X, position.Y, value)
                ));
            }
            

            SetTile (position, value);
        }

        public override void DeleteTile (IBoardIndex position) {
            T oldValue = GetTile (position);

            if (!AreEqual (oldValue, default (T))) {
                base.DeleteTile (position);
                Publish (new Record<T> (
                    RecordType.Delete,
                    new BoardState<T> (position.X, position.Y, oldValue),
                    new BoardState<T> (position.X, position.Y, default (T))
                ));
            }
        }

        public override void InsertTile (IBoardIndex position, T value) {
            T oldValue = GetTile (position);

            if (AreEqual (oldValue, default (T))) {
                base.InsertTile (position, value);
                Publish (new Record<T> (
                    RecordType.Insert,
                    new BoardState<T> (position.X, position.Y, oldValue),
                    new BoardState<T> (position.X, position.Y, value)
                ));
            }
        }

        public override void MoveTile (IBoardIndex fromPosition, IBoardIndex toPosition) {
            if (!GetTile (toPosition).Equals (default (T))) {
                return;
            }

            T value = GetTile (fromPosition);
            SetTile (toPosition, value);
            SetTile (fromPosition, default (T));
            Publish (new Record<T> (
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

            Publish (new Record<T> (
                RecordType.Merge,
                new BoardState<T> (fromPosition.X, fromPosition.Y, valueFrom),
                new BoardState<T> (toPosition.X, toPosition.Y, value)
            ));

            Publish (new Record<T> (
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
            Publish (new Record<T> (
                Record.GetOppositeRecordType (record.type),
                record.newState,
                record.oldState
            ));
        }

        /// <summary>Broadcasts a Record to any consumers added with RegisterConsumer</summary>
        public void Publish (Record<T> record) {
            publisher.Publish (record);
        }
    }
}
