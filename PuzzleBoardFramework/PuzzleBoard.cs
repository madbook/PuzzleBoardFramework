using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class PuzzleBoard<T> : RecordBoard<T>, IPushableBoard {
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
    }

}
