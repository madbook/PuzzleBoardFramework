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

        /// <summary>Insert, update, or delete the value at the given x and y coordinates.</summary>
        public void UpdateTile (int x, int y, T value) {
            UpdateTile (new Index2D (x, y), value);
        }

        /// <summary>Insert, update, or delete the value at the given Index2D position.</summary>
        public virtual void UpdateTile (Index2D position, T value) {
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

        /// <summary>Resets all MoveVectors.  Sets all values to default, using the current mergeStrategy.</summary>
        public virtual void Clear () {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    values[x,y] = default (T);
                }
            }
        }

        public bool IsPositionValue (int x, int y, T value) {
            return IsPositionValue (new Index2D (x, y), value);
        }

        public bool IsPositionValue (Index2D position, T value) {
            return GetTile (position).Equals (value);
        }

        /// <summary>Sets the value at the given Index2D position.</summary>
        protected void SetTile (Index2D position, T value) {
            if (!IsValidIndex2D (position)) {
                return;
            }
            values[position.x, position.y] = value;
        }

        /// <summary>Checks if the given Index2D is within the bounds of the PuzzleBoard</summary>
        protected bool IsValidIndex2D (Index2D index) {
            if (index.x < 0 || index.x >= width || index.y < 0 || index.y >= height) {
                return false;
            } else {
                return true;
            }
        }
    }

}
