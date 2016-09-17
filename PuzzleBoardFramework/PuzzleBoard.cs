using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class PuzzleBoard<T> : PublisherBoard<T>, ISearchableBoard<T>, IPushableBoard {
        BoardPusher<T> pushBoard;

        /// <summary>Create a new PuzzleBoard using a default MergeStrategy.</summary>
        public PuzzleBoard (int width, int height) : base (width, height) {
            MergeStrategy<T> mergeStrategy = MergeStrategy.GetDefaultStrategy<T> ();
            pushBoard = new BoardPusher<T> (width, height, this, mergeStrategy);
        }

        /// <summary>Create a new PuzzleBoard with a custom MergeStrategy.</summary>
        public PuzzleBoard (int width, int height, MergeStrategy<T> mergeStrategy) : base (width, height) {
            pushBoard = new BoardPusher<T> (width, height, this, mergeStrategy);
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

        /// <summary>Set the movement vector of the cell at the given Index2D position to the given direction.</summary>
        public void PushTile (IBoardIndex position, MoveVector push) {
            pushBoard.PushTile (position, push);
        }

        /// <summary>Set the movement vector of each cell in a list of Index2D positions to the given direction.</summary>
        public void PushTiles (List<IBoardIndex> positions, MoveVector push) {
            pushBoard.PushTiles (positions, push);
        }

        /// <summary>Set movsement vectors at all cells matching the value to the given direction.</summary>
        public void PushAllMatching (MoveVector push, T matchValue)  {
            pushBoard.PushAllMatching (push, matchValue);
        }

        /// <summary>Resets all MoveVectors.  Sets all values to default, using the current mergeStrategy.</summary>
        public override void Clear () {
            pushBoard.Clear ();
            base.Clear ();
        }

        T GetTile (int x, int y) {
            return GetTile (new BoardPosition (x, y));
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
    }

}
