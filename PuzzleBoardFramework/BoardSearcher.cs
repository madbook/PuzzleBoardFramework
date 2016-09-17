using System.Collections.Generic;

namespace PuzzleBoardFramework {

    /// <summary>Provides an ISearchableBoard interface to an existing IUpdatableBoard instance.</summary>
    public class BoardSearcher<T> : ISearchableBoard<T> {

        IUpdatableBoard<T> board;

        public BoardSearcher (IUpdatableBoard<T> board) {
            this.board = board;
        }

        /// <summary>Returns a List of Index2D positions matching the given value.</summary>
        public List<IBoardIndex> GetPositionsMatching (T matchValue) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();

            for (int y = 0; y < board.Height; y++) {
                for (int x = 0; x < board.Width; x++) {
                    if (board.IsPositionValue (new BoardPosition (x, y), matchValue)) {
                        matches.Add (new BoardPosition (x, y));
                    }
                }
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions matching any of the given values.</summary>
        public List<IBoardIndex> GetPositionsMatching (params T[] valuesToMatch) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();
            for (int y = 0; y < board.Height; y++) {
                for (int x = 0; x < board.Width; x++) {
                    foreach (T matchValue in valuesToMatch) {
                        if (board.IsPositionValue (new BoardPosition (x, y), matchValue)) {
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

            for (int x = 0; x < board.Width; x++) {
                matches.Add (new BoardPosition (x, row));
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions in the given column.</summary>
        public List<IBoardIndex> GetPositionsInColumn (int col) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();

            for (int y = 0; y < board.Height; y++) {
                matches.Add (new BoardPosition (col, y));
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions matching the given value in the given row.</summary>
        public List<IBoardIndex> GetPositionsInRowMatching (int row, T matchValue) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();

            for (int x = 0; x < board.Width; x++) {
                if (board.IsPositionValue (new BoardPosition (x, row), matchValue)) {
                    matches.Add (new BoardPosition (x, row));
                }
            }

            return matches;
        }

        /// <summary>Returns a List of Index2D positions matching the given value in the given column.</summary>
        public List<IBoardIndex> GetPositionsInColumnMatching (int col, T matchValue) {
            List<IBoardIndex> matches = new List<IBoardIndex> ();

            for (int y = 0; y < board.Height; y++) {
                if (board.IsPositionValue (new BoardPosition (col, y), matchValue)) {
                    matches.Add (new BoardPosition (col, y));
                }
            }

            return matches;
        }

        /// <summary>Get a list of all orthaganally connected positions that match the value at the given Index2D position.</summary>
        public List<IBoardIndex> GetIdenticalAdjacentPositions (T value, IBoardIndex position) {
            List<IBoardIndex> positions = new List<IBoardIndex> ();
            int[,] checkedPositions = new int[board.Width, board.Height];
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
                if (board.IsValidIndex2D (checkingUp) &&
                        checkedPositions[checkingUp.X, checkingUp.Y] == 0) {
                    checkedPositions[checkingUp.X, checkingUp.Y] = 1;
                    if (board.IsPositionValue (checkingUp, value)) {
                        positions.Add (checkingUp);
                        positionsToCheck.Enqueue (checkingUp);
                    }
                }

                BoardPosition checkingDown = checkingPosition + MoveVector.down;
                if (board.IsValidIndex2D (checkingDown) &&
                        checkedPositions[checkingDown.X, checkingDown.Y] == 0) {
                    checkedPositions[checkingDown.X, checkingDown.Y] = 1;
                    if (board.IsPositionValue (checkingDown, value)) {
                        positions.Add (checkingDown);
                        positionsToCheck.Enqueue (checkingDown);
                    }
                }

                BoardPosition checkingLeft = checkingPosition + MoveVector.left;
                if (board.IsValidIndex2D (checkingLeft) &&
                        checkedPositions[checkingLeft.X, checkingLeft.Y] == 0) {
                    checkedPositions[checkingLeft.X, checkingLeft.Y] = 1;
                    if (board.IsPositionValue (checkingLeft, value)) {
                        positions.Add (checkingLeft);
                        positionsToCheck.Enqueue (checkingLeft);
                    }
                }

                BoardPosition checkingRight = checkingPosition + MoveVector.right;
                if (board.IsValidIndex2D (checkingRight) &&
                        checkedPositions[checkingRight.X, checkingRight.Y] == 0) {
                    checkedPositions[checkingRight.X, checkingRight.Y] = 1;
                    if (board.IsPositionValue (checkingRight, value)) {
                        positions.Add (checkingRight);
                        positionsToCheck.Enqueue (checkingRight);
                    }
                }
            }

            return positions;
        }

    }

}
