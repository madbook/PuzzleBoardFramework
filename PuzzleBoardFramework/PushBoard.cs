using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public interface IPushableBoard {
        // TODO – just rename to Push so they are all one method
        void PushAll (MoveVector push);
        void PushTile (int x, int y, MoveVector push);
        void PushTile (IBoardIndex position, MoveVector push);
        void PushTile (List<IBoardIndex> positions, MoveVector push);
        // PushAllMatching skipped because only method that has type
        // same funcationality can be accomplished by passing a list of matched positions
        // to PushTile
    }

    public class PushBoard<T> : BaseBoard<MoveVector>, IPushableBoard {
        PuzzleBoard<T> board;
        MergeStrategy<T> mergeStrategy;

        public PushBoard (int width, int height, PuzzleBoard<T> board, MergeStrategy<T> mergeStrategy) : base (width, height) {
            this.board = board;
            this.mergeStrategy = mergeStrategy;
        }

        /// <summary>Set movement vectors on cells to the given direction.</summary>
        public void PushAll (MoveVector push) {
            PushAllMatching (push, default (T));
        }

        /// <summary>Set the movement vector of the cell at the given x and y coordinates to the given direction.</summary>
        public void PushTile (int x, int y, MoveVector push) {
            PushTile (new BoardPosition (x, y), push);
        }

        /// <summary>Set the movement vector of the cell at the given Index2D position to the given direction.</summary>
        public void PushTile (IBoardIndex position, MoveVector push) {
            if (IsValidIndex2D (position)) {
                UpdateTile (position, push);
            }
        }

        /// <summary>Set the movement vector of each cell in a list of Index2D positions to the given direction.</summary>
        public void PushTile (List<IBoardIndex> positions, MoveVector push) {
            foreach (IBoardIndex position in positions) {
                PushTile (position, push);
            }
        }

        /// <summary>Set movement vectors at all cells matching the value to the given direction.</summary>
        public void PushAllMatching (MoveVector push, T matchValue)  {
            if (push == MoveVector.zero) {
                return;
            }

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (board.IsPositionValue (x, y, matchValue)) {
                        UpdateTile (x, y, push);
                    }
                }
            }
        }

        /// <summary>Attempts to apply the given MoveVector to all tiles with it set.</summary>
        public void ApplyMoveVectors (MoveVector push) {
            if (push == MoveVector.left) {
                TryPushLeft ();
            } else if (push == MoveVector.right) {
                TryPushRight ();
            } else if (push == MoveVector.up) {
                TryPushUp ();
            } else if (push == MoveVector.down) {
                TryPushDown ();
            }
            Clear ();
        }

        /// <summary>Attempts to apply all valid MoveVectors to all cells on the board.</summary>
        public void ApplyMoveVectors () {
            TryPushLeft ();
            TryPushRight ();
            TryPushUp ();
            TryPushDown ();
            Clear ();
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving left.</summary>
        void TryPushLeft () {
            for (int y = 0; y < height; y++) {
                for (int x = width - 1; x >= 1; x--) {
                    MoveVector push = GetTile (x, y);
                    if (push == MoveVector.left) {
                        TryPush (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y), push);
                    }
                }

                for (int x = 1; x < width; x++) {
                    MoveVector push = GetTile (x, y);
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
                    MoveVector push = GetTile (x, y);
                    if (push == MoveVector.right) {
                        TryPush (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y), push);
                    }
                }

                for (int x = width - 2; x >= 0; x--) {
                    MoveVector push = GetTile (x, y);
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
                    MoveVector push = GetTile (x, y);
                    if (push == MoveVector.down) {
                        TryPush (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y), push);
                    }
                }

                for (int y = 1; y < height; y++) {
                    MoveVector push = GetTile (x, y);
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
                    MoveVector push = GetTile (x, y);
                    if (push == MoveVector.up) {
                        TryPush (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y), push);
                    }
                }

                for (int y = height - 2; y >= 0; y--) {
                    MoveVector push = GetTile (x, y);
                    if (push == MoveVector.up) {
                        TryMerge (new BoardPosition (x, y), new BoardPosition (x+push.x, y+push.y));
                    }
                }
            }
        }

        /// <summary>Attempts to propagate MoveVectors to stationary cells in their direction, using the set MergeStrategy.</summary>
        void TryPush (BoardPosition pushFrom, BoardPosition pushInto, MoveVector push) {
            if (!(IsValidIndex2D (pushFrom) && IsValidIndex2D (pushInto))) {
                return;
            }
            if (push == MoveVector.zero) {
                return;
            }
            T tileFrom = board.GetTile (pushFrom);
            T tileInto = board.GetTile (pushInto);
            if (mergeStrategy.IsEmpty (tileFrom)) {
                return;
            }
            if (mergeStrategy.ShouldPush (tileFrom, tileInto)) {
                UpdateTile (pushInto, push);
            }
        }

        /// <summary>Attempts to merge two cell positions, using the set MergeStrategy.</summary>
        void TryMerge (BoardPosition mergeFrom, BoardPosition mergeInto) {
            if (!(IsValidIndex2D (mergeFrom) && IsValidIndex2D (mergeInto))) {
                return;
            }
            T valueFrom = board.GetTile (mergeFrom);
            T valueInto = board.GetTile (mergeInto);
            if (valueFrom.Equals (default (T))) {
                return;
            }
            // TODO - should merge and move be explicitly diferrent in the merge strategy?
            if (mergeStrategy.ShouldMerge (valueFrom, valueInto)) {
                if (mergeStrategy.IsEmpty (valueInto)) {
                    board.MoveTile (mergeFrom, mergeInto);
                } else {
                    T newValue = mergeStrategy.Merge (valueFrom, valueInto);
                    board.MergeTile (mergeFrom, mergeInto, newValue);
                }
            }
        }
    }
}

