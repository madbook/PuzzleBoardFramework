using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class PuzzleBoard<T> : PublisherBoard<T>, ISearchableBoard<T>, IPushableBoard {

        BoardPusher<T> boardPusher;
        BoardSearcher<T> boardSearcher;

        /// <summary>Create a new PuzzleBoard using a default MergeStrategy.</summary>
        public PuzzleBoard (int width, int height) : base (width, height) {
            IMergeStrategy<T> mergeStrategy = MergeStrategy.GetDefaultStrategy<T> ();
            boardPusher = new BoardPusher<T> (this, mergeStrategy);
            boardSearcher = new BoardSearcher<T> (this);
        }

        /// <summary>Create a new PuzzleBoard with a custom MergeStrategy.</summary>
        public PuzzleBoard (int width, int height, IMergeStrategy<T> mergeStrategy) : base (width, height) {
            boardPusher = new BoardPusher<T> (this, mergeStrategy);
            boardSearcher = new BoardSearcher<T> (this);
        }

        /// <summary>Try to move each cell by it's currently set move vector, then reset all move vectors.</summary>
        public void ApplyMovementAndReset () {
            boardPusher.ApplyMoveVectors ();
        }

        /// <summary>Try to move each cell with the given MoveVector set, then reset all move vectors.</summary>
        public void ApplyMovementAndReset (MoveVector push) {
            boardPusher.ApplyMoveVectors (push);
        }

        /// <summary>Set movement vectors on cells to the given direction.</summary>
        public void PushAll (MoveVector push) {
            boardPusher.PushAll (push);
        }

        /// <summary>Set the movement vector of the cell at the given Index2D position to the given direction.</summary>
        public void PushTile (IBoardIndex position, MoveVector push) {
            boardPusher.PushTile (position, push);
        }

        /// <summary>Set the movement vector of each cell in a list of Index2D positions to the given direction.</summary>
        public void PushTiles (List<IBoardIndex> positions, MoveVector push) {
            boardPusher.PushTiles (positions, push);
        }

        /// <summary>Set movsement vectors at all cells matching the value to the given direction.</summary>
        public void PushAllMatching (MoveVector push, T matchValue)  {
            boardPusher.PushAllMatching (push, matchValue);
        }

        /// <summary>Resets all MoveVectors.  Sets all values to default, using the current mergeStrategy.</summary>
        public override void Clear () {
            boardPusher.Clear ();
            base.Clear ();
        }

        public List<IBoardIndex> GetPositionsMatching (T matchValue) {
            return boardSearcher.GetPositionsMatching (matchValue);
        }

        public List<IBoardIndex> GetPositionsMatching (params T[] valuesToMatch) {
            return boardSearcher.GetPositionsMatching (valuesToMatch);
        }

        public List<IBoardIndex> GetPositionsInRow (int row) {
            return boardSearcher.GetPositionsInRow (row);
        }

        public List<IBoardIndex> GetPositionsInColumn (int col) {
            return boardSearcher.GetPositionsInColumn (col);
        }

        public List<IBoardIndex> GetPositionsInRowMatching (int row, T matchValue) {
            return boardSearcher.GetPositionsInRowMatching (row, matchValue);
        }

        public List<IBoardIndex> GetPositionsInColumnMatching (int col, T matchValue) {
            return boardSearcher.GetPositionsInColumnMatching (col, matchValue);
        }

        public List<IBoardIndex> GetIdenticalAdjacentPositions (T value, IBoardIndex position) {
            return boardSearcher.GetIdenticalAdjacentPositions (value, position);
        }

    }

}
