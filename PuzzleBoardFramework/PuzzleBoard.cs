using System;
using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class PuzzleBoard<T> {
        public readonly int width;
        public readonly int height;

        MergeStrategy<T> mergeStrategy;
        T[,] values;
        MoveVector[,] moveVectors;
        Publisher<Record<T>> publisher = new Publisher<Record<T>> ();

        /// <summary>Create a new PuzzleBoard using a default MergeStrategy.</summary>
        public PuzzleBoard (int width, int height) {
            this.width = width;
            this.height = height;
            this.mergeStrategy = MergeStrategy.GetDefaultStrategy<T> ();
            Init ();
        }

        /// <summary>Create a new PuzzleBoard with a custom MergeStrategy.</summary>
        public PuzzleBoard (int width, int height, MergeStrategy<T> mergeStrategy) {
            this.width = width;
            this.height = height;
            this.mergeStrategy = mergeStrategy;
            Init ();
        }

        void Init () {
            values = new T[width,height];
            moveVectors = new MoveVector[width,height];
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
            if (push == MoveVector.zero) {
                return;
            }

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (!mergeStrategy.IsEmpty (values[x,y])) {
                        moveVectors[x,y] = push;
                    }
                }
            }
        }

        /// <summary>Set movement vectors at all cells matching the value to the given direction.</summary>
        public void PushAllMatching (MoveVector push, T matchValue)  {
            if (push == MoveVector.zero) {
                return;
            }

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (values[x,y].Equals(matchValue)) {
                        moveVectors[x,y] = push;
                    }
                }
            }
        }

        /// <summary>Set the movement vector of the cell at the given x and y coordinates to the given direction.</summary>
        public void PushTile (int x, int y, MoveVector push) {
            PushTile (new Index2D (x, y), push);
        }

        /// <summary>Set the movement vector of the cell at the given Index2D position to the given direction.</summary>
        public void PushTile (Index2D position, MoveVector push) {
            if (IsValidIndex2D (position)) {
                moveVectors[position.x, position.y] = push;
            }
        }

        /// <summary>Set the movement vector of each cell in a list of Index2D positions to the given direction.</summary>
        public void PushTile (List<Index2D> positions, MoveVector push) {
            foreach (Index2D position in positions) {
                PushTile (position, push);
            }
        }

        /// <summary>Insert, update, or delete the value at the given x and y coordinates.</summary>
        public void UpdateTile (int x, int y, T value) {
            UpdateTile (new Index2D (x, y), value);
        }

        /// <summary>Insert, update, or delete the value at the given Index2D position.</summary>
        public void UpdateTile (Index2D position, T value) {
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
                position,
                position,
                oldValue,
                value
            ));

            SetTile (position, value);
        }

        /// <summary>Insert, update, or delete each value in a list of Index2D positions.</summary> 
        public void UpdateTile (List<Index2D> positions, T value) {
            foreach (Index2D position in positions) {
                UpdateTile (position, value);
            }
        }

        /// <summary>Returns the value at the given x and y coordinates.</summary>
        public T GetTile (int x, int y) {
            return values[x, y];
        }

        /// <summary>Returns the value at the given Index2D position.</summary>
        public T GetTile (Index2D position) {
            return values[position.x, position.y];
        }

        /// <summary>Returns a List of Index2D positions matching the given value.</summary>
        public List<Index2D> GetPositionsMatching (T matchValue) {
            List<Index2D> matches = new List<Index2D> ();

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (values[x,y].Equals (matchValue)) {
                        matches.Add (new Index2D (x, y));
                    }
                }
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions matching any of the given values.</summary>
        public List<Index2D> GetPositionsMatching (params T[] valuesToMatch) {
            List<Index2D> matches = new List<Index2D> ();
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    foreach (T matchValue in valuesToMatch) {
                        if (values[x,y].Equals (matchValue)) {
                            matches.Add (new Index2D (x, y));
                            goto Next;
                        }
                    }
                    Next:;
                }
            }
            return matches;
        }

        /// <summary>Returns a List of Index2D positions in the given row.</summary>
        public List<Index2D> GetPositionsInRow (int row) {
            List<Index2D> matches = new List<Index2D> ();

            for (int x = 0; x < width; x++) {
                matches.Add (new Index2D (x, row));
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions in the given column.</summary>
        public List<Index2D> GetPositionsInColumn (int col) {
            List<Index2D> matches = new List<Index2D> ();

            for (int y = 0; y < height; y++) {
                matches.Add (new Index2D (col, y));
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions matching the given value in the given row.</summary>
        public List<Index2D> GetPositionsInRowMatching (int row, T matchValue) {
            List<Index2D> matches = new List<Index2D> ();

            for (int x = 0; x < width; x++) {
                if (values[x,row].Equals (matchValue)) {
                    matches.Add (new Index2D (x, row));
                }
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions matching the given value in the given column.</summary>
        public List<Index2D> GetPositionsInColumnMatching (int col, T matchValue) {
            List<Index2D> matches = new List<Index2D> ();

            for (int y = 0; y < height; y++) {
                if (values[col, y].Equals (matchValue)) {
                    matches.Add (new Index2D (col, y));
                }
            }

            return matches;
        }

        /// <summary>Get a list of all orthaganally connected positions that match the value at the given x and y coordinates.</summary>
        public List<Index2D> GetIdenticalAdjacentPositions (T value, int x, int y) {
            return GetIdenticalAdjacentPositions (value, new Index2D (x, y));
        }

        /// <summary>Get a list of all orthaganally connected positions that match the value at the given Index2D position.</summary>
        public List<Index2D> GetIdenticalAdjacentPositions (T value, Index2D position) {
            List<Index2D> positions = new List<Index2D> ();
            int[,] checkedPositions = new int[width, height];
            Queue<Index2D> positionsToCheck = new Queue<Index2D> ();

            checkedPositions[position.x, position.y] = 1;
            positions.Add (position);
            positionsToCheck.Enqueue (position);

            // Each item in the queue should already be in the positions List.
            while (positionsToCheck.Count > 0) {
                Index2D checkingPosition = positionsToCheck.Dequeue ();

                Index2D checkingUp = checkingPosition + MoveVector.up;
                if (IsValidIndex2D (checkingUp) &&
                        checkedPositions[checkingUp.x, checkingUp.y] == 0) {
                    checkedPositions[checkingUp.x, checkingUp.y] = 1;
                    if (values[checkingUp.x, checkingUp.y].Equals(value)) {
                        positions.Add (checkingUp);
                        positionsToCheck.Enqueue (checkingUp);
                    }
                }

                Index2D checkingDown = checkingPosition + MoveVector.down;
                if (IsValidIndex2D (checkingDown) &&
                        checkedPositions[checkingDown.x, checkingDown.y] == 0) {
                    checkedPositions[checkingDown.x, checkingDown.y] = 1;
                    if (values[checkingDown.x, checkingDown.y].Equals(value)) {
                        positions.Add (checkingDown);
                        positionsToCheck.Enqueue (checkingDown);
                    }
                }

                Index2D checkingLeft = checkingPosition + MoveVector.left;
                if (IsValidIndex2D (checkingLeft) &&
                        checkedPositions[checkingLeft.x, checkingLeft.y] == 0) {
                    checkedPositions[checkingLeft.x, checkingLeft.y] = 1;
                    if (values[checkingLeft.x, checkingLeft.y].Equals(value)) {
                        positions.Add (checkingLeft);
                        positionsToCheck.Enqueue (checkingLeft);
                    }
                }

                Index2D checkingRight = checkingPosition + MoveVector.right;
                if (IsValidIndex2D (checkingRight) &&
                        checkedPositions[checkingRight.x, checkingRight.y] == 0) {
                    checkedPositions[checkingRight.x, checkingRight.y] = 1;
                    if (values[checkingRight.x, checkingRight.y].Equals(value)) {
                        positions.Add (checkingRight);
                        positionsToCheck.Enqueue (checkingRight);
                    }
                }
            }

            return positions;
        }

        public void UndoRecord (Record<T> record) {
            if (!IsPositionValue (record.newPosition, record.newValue)) {
                return;
            }

            if (record.type == RecordType.Move) {
                if (!IsPositionValue (record.oldPosition, mergeStrategy.Empty ())) {
                    return;
                }
                values [record.newPosition.x, record.newPosition.y] = mergeStrategy.Empty ();
            }

            values [record.oldPosition.x, record.oldPosition.y] = record.oldValue;
            AddRecord (new Record<T> (
                Record.GetOppositeRecordType (record.type),
                record.newPosition,
                record.oldPosition,
                record.newValue,
                record.oldValue
            ));
        }

        /// <summary>Resets all MoveVectors.  Sets all values to default, using the current mergeStrategy.</summary>
        public void Clear () {
            ClearMoveVectors ();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    values[x,y] = mergeStrategy.Empty ();
                }
            }
        }

        bool IsPositionValue (Index2D position, T value) {
            return GetTile (position).Equals (value);
        }

        /// <summary>Sets the value at the given Index2D position.</summary>
        void SetTile (Index2D position, T value) {
            if (!IsValidIndex2D (position)) {
                return;
            }
            values[position.x, position.y] = value;
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
                    MoveVector push = moveVectors[x,y];
                    if (push == MoveVector.left) {
                        TryPush (new Index2D (x, y), new Index2D (x+push.x, y+push.y), push);
                    }
                }

                for (int x = 1; x < width; x++) {
                    MoveVector push = moveVectors[x,y];
                    if (push == MoveVector.left) {
                        TryMerge (new Index2D (x, y), new Index2D (x+push.x, y+push.y));
                    }
                }
            }
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving right.</summary>
        void TryPushRight () {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width - 1; x++) {
                    MoveVector push = moveVectors[x,y];
                    if (push == MoveVector.right) {
                        TryPush (new Index2D (x, y), new Index2D (x+push.x, y+push.y), push);
                    }
                }

                for (int x = width - 2; x >= 0; x--) {
                    MoveVector push = moveVectors[x,y];
                    if (push == MoveVector.right) {
                        TryMerge (new Index2D (x, y), new Index2D (x+push.x, y+push.y));
                    }
                }
            }
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving down.</summary>
        void TryPushDown () {
            for (int x = 0; x < width; x++) {
                for (int y = height - 1; y >= 1; y--) {
                    MoveVector push = moveVectors[x,y];
                    if (push == MoveVector.down) {
                        TryPush (new Index2D (x, y), new Index2D (x+push.x, y+push.y), push);
                    }
                }

                for (int y = 1; y < height; y++) {
                    MoveVector push = moveVectors[x,y];
                    if (push == MoveVector.down) {
                        TryMerge (new Index2D (x, y), new Index2D (x+push.x, y+push.y));
                    }
                }
            }
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving up.</summary>  
        void TryPushUp () {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height - 1; y++) {
                    MoveVector push = moveVectors[x,y];
                    if (push == MoveVector.up) {
                        TryPush (new Index2D (x, y), new Index2D (x+push.x, y+push.y), push);
                    }
                }

                for (int y = height - 2; y >= 0; y--) {
                    MoveVector push = moveVectors[x,y];
                    if (push == MoveVector.up) {
                        TryMerge (new Index2D (x, y), new Index2D (x+push.x, y+push.y));
                    }
                }
            }
        }

        /// <summary>Resest all of the MoveVector values to MoveVector.zero</summary>
        void ClearMoveVectors () {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    moveVectors[x,y] = MoveVector.zero;
                }
            }
        }

        /// <summary>Attempts to propagate MoveVectors to stationary cells in their direction, using the set MergeStrategy.</summary>
        void TryPush (Index2D pushFrom, Index2D pushInto, MoveVector push) {
            if (!(IsValidIndex2D (pushFrom) && IsValidIndex2D (pushInto))) {
                return;
            }
            if (push == MoveVector.zero) {
                return;
            }
            T tileFrom = values[pushFrom.x, pushFrom.y];
            T tileInto = values[pushInto.x, pushInto.y];
            if (mergeStrategy.IsEmpty (tileFrom)) {
                return;
            }
            if (mergeStrategy.ShouldPush (tileFrom, tileInto)) {
                moveVectors[pushInto.x, pushInto.y] = push;
            }
        }

        /// <summary>Attempts to merge two cell positions, using the set MergeStrategy.</summary>
        void TryMerge (Index2D mergeFrom, Index2D mergeInto) {
            if (!(IsValidIndex2D (mergeFrom) && IsValidIndex2D (mergeInto))) {
                return;
            }
            T valueFrom = values[mergeFrom.x, mergeFrom.y];
            T valueInto = values[mergeInto.x, mergeInto.y];
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
                        mergeFrom,
                        mergeInto,
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
                        mergeFrom,
                        mergeInto,
                        valueFrom,
                        newTile
                    ));

                    AddRecord (new Record<T> (
                        RecordType.Merge,
                        mergeInto,
                        mergeInto,
                        valueInto,
                        newTile
                    ));
                }
            }
        }

        /// <summary>Checks if the given Index2D is within the bounds of the PuzzleBoard</summary>
        bool IsValidIndex2D (Index2D index) {
            if (index.x < 0 || index.x >= width || index.y < 0 || index.y >= height) {
                return false;
            } else {
                return true;
            }
        }
    }

}
