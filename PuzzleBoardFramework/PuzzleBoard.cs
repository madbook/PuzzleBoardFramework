using System;
using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class PuzzleBoard<T> : BaseBoard<T> {
        MergeStrategy<T> mergeStrategy;
        BaseBoard<MoveVector> moveVectors;
        Publisher<Record<T>> publisher = new Publisher<Record<T>> ();

        /// <summary>Create a new PuzzleBoard using a default MergeStrategy.</summary>
        public PuzzleBoard (int width, int height) : base (width, height) {
            this.mergeStrategy = MergeStrategy.GetDefaultStrategy<T> ();
            moveVectors = new BaseBoard<MoveVector> (width, height);
        }

        /// <summary>Create a new PuzzleBoard with a custom MergeStrategy.</summary>
        public PuzzleBoard (int width, int height, MergeStrategy<T> mergeStrategy) : base (width, height) {
            this.mergeStrategy = mergeStrategy;
            moveVectors = new BaseBoard<MoveVector> (width, height);
        }

        /// <summary>Register a callback method to be called when a Record is added via AddRecord.</summary>
        public void RegisterConsumer (Action<Record<T>> callback) {
            // TODO move this out into the controller
            publisher.Subscribe (callback);
        }

        /// <summary>Try to move each cell by it's currently set move vector, then reset all move vectors.</summary>
        public void ApplyMovementAndReset () {
            ApplyMoveVectors ();
            ClearMoveVectors ();
        }

        /// <summary>Try to move each cell with the given MoveVector set, then reset all move vectors.</summary>
        public void ApplyMovementAndReset (MoveVector push) {
            ApplyMoveVectors (push);
            ClearMoveVectors ();
        }

        /// <summary>Set movement vectors on cells to the given direction.</summary>
        public void PushAll (MoveVector push) {
            PushAllMatching (push, default (T));
        }

        /// <summary>Set movement vectors at all cells matching the value to the given direction.</summary>
        public void PushAllMatching (MoveVector push, T matchValue)  {
            if (push == MoveVector.zero) {
                return;
            }

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (IsPositionValue (x, y, matchValue)) {
                        moveVectors.UpdateTile (x, y, push);
                    }
                }
            }
        }

        /// <summary>Set the movement vector of the cell at the given x and y coordinates to the given direction.</summary>
        public void PushTile (int x, int y, MoveVector push) {
            PushTile (new BoardPosition (x, y), push);
        }

        /// <summary>Set the movement vector of the cell at the given Index2D position to the given direction.</summary>
        public void PushTile (IBoardIndex position, MoveVector push) {
            if (IsValidIndex2D (position)) {
                moveVectors.UpdateTile (position, push);
            }
        }

        /// <summary>Set the movement vector of each cell in a list of Index2D positions to the given direction.</summary>
        public void PushTile (List<IBoardIndex> positions, MoveVector push) {
            foreach (IBoardIndex position in positions) {
                PushTile (position, push);
            }
        }

        /// <summary>Insert, update, or delete the value at the given Index2D position.</summary>
        public override void UpdateTile (IBoardIndex position, T value) {
            T oldValue = GetTile (position);

            bool newIsEmpty = mergeStrategy.IsEmpty (value);
            bool oldIsEmpty = mergeStrategy.IsEmpty (oldValue);

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
            ClearMoveVectors ();
            base.Clear ();
        }

        /// <summary>Broadcasts a Record to any consumers added with RegisterConsumer</summary>
        void AddRecord (Record<T> record) {
            publisher.Publish (record);
        }

        /// <summary>Attempts to apply the given MoveVector to all tiles with it set.</summary>
        void ApplyMoveVectors (MoveVector push) {
            if (push == MoveVector.left) {
                TryPushLeft ();
            } else if (push == MoveVector.right) {
                TryPushRight ();
            } else if (push == MoveVector.up) {
                TryPushUp ();
            } else if (push == MoveVector.down) {
                TryPushDown ();
            }
        }

        /// <summary>Attempts to apply all valid MoveVectors to all cells on the board.</summary>
        void ApplyMoveVectors () {
            TryPushLeft ();
            TryPushRight ();
            TryPushUp ();
            TryPushDown ();
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving left.</summary>
        void TryPushLeft () {
            for (int y = 0; y < height; y++) {
                for (int x = width - 1; x >= 1; x--) {
                    MoveVector push = moveVectors.GetTile (x, y);
                    if (push == MoveVector.left) {
                        TryPush (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y), push);
                    }
                }

                for (int x = 1; x < width; x++) {
                    MoveVector push = moveVectors.GetTile (x, y);
                    if (push == MoveVector.left) {
                        TryMerge (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y));
                    }
                }
            }
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving right.</summary>
        void TryPushRight () {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width - 1; x++) {
                    MoveVector push = moveVectors.GetTile (x, y);
                    if (push == MoveVector.right) {
                        TryPush (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y), push);
                    }
                }

                for (int x = width - 2; x >= 0; x--) {
                    MoveVector push = moveVectors.GetTile (x, y);
                    if (push == MoveVector.right) {
                        TryMerge (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y));
                    }
                }
            }
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving down.</summary>
        void TryPushDown () {
            for (int x = 0; x < width; x++) {
                for (int y = height - 1; y >= 1; y--) {
                    MoveVector push = moveVectors.GetTile (x, y);
                    if (push == MoveVector.down) {
                        TryPush (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y), push);
                    }
                }

                for (int y = 1; y < height; y++) {
                    MoveVector push = moveVectors.GetTile (x, y);
                    if (push == MoveVector.down) {
                        TryMerge (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y));
                    }
                }
            }
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving up.</summary>  
        void TryPushUp () {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height - 1; y++) {
                    MoveVector push = moveVectors.GetTile (x, y);
                    if (push == MoveVector.up) {
                        TryPush (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y), push);
                    }
                }

                for (int y = height - 2; y >= 0; y--) {
                    MoveVector push = moveVectors.GetTile (x, y);
                    if (push == MoveVector.up) {
                        TryMerge (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y));
                    }
                }
            }
        }

        /// <summary>Resest all of the MoveVector values to MoveVector.zero</summary>
        void ClearMoveVectors () {
            moveVectors.Clear ();
        }

        /// <summary>Attempts to propagate MoveVectors to stationary cells in their direction, using the set MergeStrategy.</summary>
        void TryPush (BoardPosition pushFrom, BoardPosition pushInto, MoveVector push) {
            if (!(IsValidIndex2D (pushFrom) && IsValidIndex2D (pushInto))) {
                return;
            }
            if (push == MoveVector.zero) {
                return;
            }
            T tileFrom = GetTile (pushFrom);
            T tileInto = GetTile (pushInto);
            if (mergeStrategy.IsEmpty (tileFrom)) {
                return;
            }
            if (mergeStrategy.ShouldPush (tileFrom, tileInto)) {
                moveVectors.UpdateTile (pushInto, push);
            }
        }

        /// <summary>Attempts to merge two cell positions, using the set MergeStrategy.</summary>
        void TryMerge (BoardPosition mergeFrom, BoardPosition mergeInto) {
            if (!(IsValidIndex2D (mergeFrom) && IsValidIndex2D (mergeInto))) {
                return;
            }
            T valueFrom = GetTile (mergeFrom);
            T valueInto = GetTile (mergeInto);
            if (mergeStrategy.IsEmpty (valueFrom)) {
                return;
            }
            // TODO - should merge and move be explicitly diferrent in the merge strategy?
            if (mergeStrategy.ShouldMerge (valueFrom, valueInto)) {
                if (mergeStrategy.IsEmpty (valueInto)) {
                    SetTile (mergeInto, valueFrom);
                    SetTile (mergeFrom, valueInto);

                    AddRecord (new Record<T> (
                        RecordType.Move,
                        new BoardPosition (mergeFrom.X, mergeFrom.Y),
                        new BoardPosition (mergeInto.X, mergeInto.Y),
                        valueFrom,
                        valueFrom
                    ));
                } else {
                    T newTile = mergeStrategy.Merge (valueFrom, valueInto);
                    T emptyTile = mergeStrategy.Empty ();

                    SetTile (mergeInto, newTile);
                    SetTile (mergeFrom, emptyTile);

                    AddRecord (new Record<T> (
                        RecordType.Merge,
                        new BoardPosition (mergeFrom.X, mergeFrom.Y),
                        new BoardPosition (mergeInto.X, mergeInto.Y),
                        valueFrom,
                        newTile
                    ));

                    AddRecord (new Record<T> (
                        RecordType.Merge,
                        new BoardPosition (mergeInto.X, mergeInto.Y),
                        new BoardPosition (mergeInto.X, mergeInto.Y),
                        valueInto,
                        newTile
                    ));
                }
            }
        }
    }

}
