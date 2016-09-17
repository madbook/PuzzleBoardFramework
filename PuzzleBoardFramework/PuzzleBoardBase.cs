using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class BaseBoard<T> {
        public readonly int width;
        public readonly int height;

        T[,] values;

        /// <summary>Create a new PuzzleBoard using a default MergeStrategy.</summary>
        public BaseBoard (int width, int height) {
            this.width = width;
            this.height = height;
            values = new T[width,height];
        }

        /// <summary>Insert, update, or delete each value in a list of Index2D positions.</summary> 
        public void UpdateTiles (List<IBoardIndex> positions, T value) {
            foreach (IBoardIndex position in positions) {
                UpdateTile (position, value);
            }
        }

        /// <summary>Insert, update, or delete the value at the given Index2D position.</summary>
        public virtual void UpdateTile (IBoardIndex position, T value) {
            SetTile (position, value);
        }

        public virtual void MoveTile (IBoardIndex fromPosition, IBoardIndex toPosition) {
            if (!AreEqual (GetTile (toPosition), default (T))) {
                return;
            }

            T value = GetTile (fromPosition);
            SetTile (toPosition, value);
            SetTile (fromPosition, default (T));
        }

        public virtual void MergeTile (IBoardIndex fromPosition, IBoardIndex toPosition, T value) {
            SetTile (toPosition, value);
            SetTile (fromPosition, default (T));
        }

        T GetTile (int x, int y) {
            return values[x, y];
        }

        /// <summary>Returns the value at the given Index2D position.</summary>
        public T GetTile (IBoardIndex position) {
            return values[position.X, position.Y];
        }

        /// <summary>Returns a List of Index2D positions matching the given value.</summary>
        public List<IBoardIndex> GetPositionsMatching (T matchValue) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (AreEqual (GetTile (x, y), matchValue)) {
                        matches.Add (new BoardPosition (x, y));
                    }
                }
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions matching any of the given values.</summary>
        public List<IBoardIndex> GetPositionsMatching (params T[] valuesToMatch) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    foreach (T matchValue in valuesToMatch) {
                        if (AreEqual (GetTile (x, y), matchValue)) {
                            matches.Add (new BoardPosition (x, y));
                            goto Next;
                        }
                    }
                    Next:;
                }
            }
            return matches;
        }

        /// <summary>Returns a List of Index2D positions in the given row.</summary>
        public List<IBoardIndex> GetPositionsInRow (int row) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();

            for (int x = 0; x < width; x++) {
                matches.Add (new BoardPosition (x, row));
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions in the given column.</summary>
        public List<IBoardIndex> GetPositionsInColumn (int col) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();

            for (int y = 0; y < height; y++) {
                matches.Add (new BoardPosition (col, y));
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions matching the given value in the given row.</summary>
        public List<IBoardIndex> GetPositionsInRowMatching (int row, T matchValue) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();

            for (int x = 0; x < width; x++) {
                if (AreEqual (GetTile (x, row), matchValue)) {
                    matches.Add (new BoardPosition (x, row));
                }
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions matching the given value in the given column.</summary>
        public List<IBoardIndex> GetPositionsInColumnMatching (int col, T matchValue) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();

            for (int y = 0; y < height; y++) {
                if (AreEqual (GetTile (col, y), matchValue)) {
                    matches.Add (new BoardPosition (col, y));
                }
            }

            return matches;
        }

        /// <summary>Get a list of all orthaganally connected positions that match the value at the given Index2D position.</summary>
        public List<IBoardIndex> GetIdenticalAdjacentPositions (T value, IBoardIndex position) {
            List<IBoardIndex> positions = new List<IBoardIndex> ();
            int[,] checkedPositions = new int[width, height];
            Queue<BoardPosition> positionsToCheck = new Queue<BoardPosition> ();
            // is there an easier way to do this??
            BoardPosition _position = new BoardPosition (position.X, position.Y);
            checkedPositions[position.X, position.Y] = 1;
            positions.Add (position);
            positionsToCheck.Enqueue (_position);

            // Each item in the queue should already be in the positions List.
            while (positionsToCheck.Count > 0) {
                BoardPosition checkingPosition = positionsToCheck.Dequeue ();

                BoardPosition checkingUp = checkingPosition + MoveVector.up;
                if (IsValidIndex2D (checkingUp) &&
                        checkedPositions[checkingUp.X, checkingUp.Y] == 0) {
                    checkedPositions[checkingUp.X, checkingUp.Y] = 1;
                    if (AreEqual (GetTile (checkingUp), value)) {
                        positions.Add (checkingUp);
                        positionsToCheck.Enqueue (checkingUp);
                    }
                }

                BoardPosition checkingDown = checkingPosition + MoveVector.down;
                if (IsValidIndex2D (checkingDown) &&
                        checkedPositions[checkingDown.X, checkingDown.Y] == 0) {
                    checkedPositions[checkingDown.X, checkingDown.Y] = 1;
                    if (AreEqual (GetTile (checkingDown), value)) {
                        positions.Add (checkingDown);
                        positionsToCheck.Enqueue (checkingDown);
                    }
                }

                BoardPosition checkingLeft = checkingPosition + MoveVector.left;
                if (IsValidIndex2D (checkingLeft) &&
                        checkedPositions[checkingLeft.X, checkingLeft.Y] == 0) {
                    checkedPositions[checkingLeft.X, checkingLeft.Y] = 1;
                    if (AreEqual (GetTile (checkingLeft), value)) {
                        positions.Add (checkingLeft);
                        positionsToCheck.Enqueue (checkingLeft);
                    }
                }

                BoardPosition checkingRight = checkingPosition + MoveVector.right;
                if (IsValidIndex2D (checkingRight) &&
                        checkedPositions[checkingRight.X, checkingRight.Y] == 0) {
                    checkedPositions[checkingRight.X, checkingRight.Y] = 1;
                    if (AreEqual (GetTile (checkingRight), value)) {
                        positions.Add (checkingRight);
                        positionsToCheck.Enqueue (checkingRight);
                    }
                }
            }

            return positions;
        }

        /// <summary>Resets all MoveVectors.  Sets all values to default, using the current mergeStrategy.</summary>
        public virtual void Clear () {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    values[x,y] = default (T);
                }
            }
        }

        public bool IsPositionValue (IBoardIndex position, T value) {
            return AreEqual (GetTile (position), value);
        }

        /// <summary>Sets the value at the given Index2D position.</summary>
        protected void SetTile (IBoardIndex position, T value) {
            if (!IsValidIndex2D (position)) {
                return;
            }
            values[position.X, position.Y] = value;
        }

        /// <summary>Checks if the given Index2D is within the bounds of the PuzzleBoard</summary>
        protected bool IsValidIndex2D (IBoardIndex index) {
            if (index.X < 0 || index.X >= width || index.Y < 0 || index.Y >= height) {
                return false;
            } else {
                return true;
            }
        }

        protected bool AreEqual (T valueA, T valueB) {
            return EqualityComparer<T>.Default.Equals (valueA, valueB);
        }
    }

}
