using System.Collections.Generic;

namespace PuzzleBoardFramework {

    /// <summary>Provides an IPushableBoard interface to an existing IUpdatableBoard instance.</summary>
    public class BoardPusher<T> : BaseBoard<MoveVector>, IBoardPusher<T>, IPushStrategy<T> {

        IMovableBoard<T> board;
        IPushStrategy<T> mergeController;

        public BoardPusher (IMovableBoard<T> board) : base (board.Width, board.Height) {
            this.board = board;
            this.mergeController = this;
        }

        public BoardPusher (IMovableBoard<T> board, IPushStrategy<T> mergeController) : base (board.Width, board.Height) {
            this.board = board;
            this.mergeController = mergeController;
        }

        /// <summary>Set movement vectors on cells to the given direction.</summary>
        public void PushAll (MoveVector push) {
            if (push == MoveVector.zero) {
                return;
            }

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    BoardPosition position = new BoardPosition(x, y);
                    UpdateTile (position, push);
                }
            }
        }

        /// <summary>Set the movement vector of the cell at the given Index2D position to the given direction.</summary>
        public void PushTile (IBoardIndex position, MoveVector push) {
            if (IsValidIndex2D (position)) {
                UpdateTile (position, push);
            }
        }

        /// <summary>Set the movement vector of each cell in a list of Index2D positions to the given direction.</summary>
        public void PushTiles (List<IBoardIndex> positions, MoveVector push) {
            foreach (IBoardIndex position in positions) {
                PushTile (position, push);
            }
        }

        /// <summary>Set movement vectors at all cells matching the value to the given direction.</summary>
        public void PushAllMatching (MoveVector push, T matchValue)  {
            if (push == MoveVector.zero) {
                return;
            }

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    BoardPosition position = new BoardPosition(x, y);
                    if (board.IsPositionValue (position, matchValue)) {
                        UpdateTile (position, push);
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

        public virtual bool ShouldMove (IBoardIndex from, IBoardIndex into) {
            return BaseBoard<T>.IsEmpty (board.GetTile (into));
        }

        public virtual bool ShouldPush (IBoardIndex from, IBoardIndex into) {
            return !BaseBoard<T>.IsEmpty (board.GetTile (into));
        }

        public virtual bool ShouldMerge (IBoardIndex from, IBoardIndex into) {
            return false;
        }

        public virtual T GetMergedValue (IBoardIndex from, IBoardIndex into) {
            return board.GetTile (from);
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving left.</summary>
        void TryPushLeft () {
            for (int y = 0; y < Height; y++) {
                for (int x = Width - 1; x >= 1; x--) {
                    BoardPosition position = new BoardPosition (x, y);
                    MoveVector push = GetTile (position);
                    if (push == MoveVector.left) {
                        TryPush (new BoardPosition (x, y), position + push, push);
                    }
                }

                for (int x = 1; x < Width; x++) {
                    BoardPosition position = new BoardPosition (x, y);
                    MoveVector push = GetTile (position);
                    if (push == MoveVector.left) {
                        TryMerge (new BoardPosition (x, y), position + push);
                    }
                }
            }
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving right.</summary>
        void TryPushRight () {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width - 1; x++) {
                    BoardPosition position = new BoardPosition (x, y);
                    MoveVector push = GetTile (position);
                    if (push == MoveVector.right) {
                        TryPush (new BoardPosition (x, y), position + push, push);
                    }
                }

                for (int x = Width - 2; x >= 0; x--) {
                    BoardPosition position = new BoardPosition (x, y);
                    MoveVector push = GetTile (position);
                    if (push == MoveVector.right) {
                        TryMerge (new BoardPosition (x, y), position + push);
                    }
                }
            }
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving down.</summary>
        void TryPushDown () {
            for (int x = 0; x < Width; x++) {
                for (int y = Height - 1; y >= 1; y--) {
                    BoardPosition position = new BoardPosition (x, y);
                    MoveVector push = GetTile (position);
                    if (push == MoveVector.down) {
                        TryPush (new BoardPosition (x, y), position + push, push);
                    }
                }

                for (int y = 1; y < Height; y++) {
                    BoardPosition position = new BoardPosition (x, y);
                    MoveVector push = GetTile (position);
                    if (push == MoveVector.down) {
                        TryMerge (new BoardPosition (x, y), position + push);
                    }
                }
            }
        }

        /// <summary>Iterates through all cells and attempts to apply movement to those currently moving up.</summary>  
        void TryPushUp () {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height - 1; y++) {
                    BoardPosition position = new BoardPosition (x, y);
                    MoveVector push = GetTile (position);
                    if (push == MoveVector.up) {
                        TryPush (new BoardPosition (x, y), position + push, push);
                    }
                }

                for (int y = Height - 2; y >= 0; y--) {
                    BoardPosition position = new BoardPosition (x, y);
                    MoveVector push = GetTile (position);
                    if (push == MoveVector.up) {
                        TryMerge (new BoardPosition (x, y), position + push);
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
            // TODO - this should be part of ShouldPush (i think)
            T tileFrom = board.GetTile (pushFrom);
            if (BaseBoard<T>.IsEmpty (tileFrom)) {
                return;
            }
            if (mergeController.ShouldPush (pushFrom, pushInto)) {
                UpdateTile (pushInto, push);
            }
        }

        /// <summary>Attempts to merge two cell positions, using the set MergeStrategy.</summary>
        void TryMerge (BoardPosition mergeFrom, BoardPosition mergeInto) {
            if (!(IsValidIndex2D (mergeFrom) && IsValidIndex2D (mergeInto))) {
                return;
            }
            // TODO - this should be part of ShouldMove and ShouldMerge (i think)
            T valueFrom = board.GetTile (mergeFrom);
            T valueInto = board.GetTile (mergeInto);
            if (BaseBoard<T>.AreEqual (valueFrom, default (T))) {
                return;
            }
            if (mergeController.ShouldMove (mergeFrom, mergeInto)) {
                board.MoveTile (mergeFrom, mergeInto);
            } else if (mergeController.ShouldMerge (mergeFrom, mergeInto)) {
                T newValue = mergeController.GetMergedValue (mergeFrom, mergeInto);
                board.MergeTile (mergeFrom, mergeInto, newValue);
            }
        }

    }

}
