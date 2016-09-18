using System.Collections.Generic;

namespace PuzzleBoardFramework {

    /// <summary>A basic IUpdatableBoard implementation.</summary>
    public class BaseBoard<T> : IUpdatableBoard<T> {
        readonly int width;
        readonly int height;

        T[,] values;

        /// <summary>Create a new PuzzleBoard using a default MergeStrategy.</summary>
        public BaseBoard (int width, int height) {
            this.width = width;
            this.height = height;
            values = new T[width,height];
        }

        public int Width {
            get { return width; }
        }

        public int Height {
            get { return height; }
        }

        /// <summary>Insert, update, or delete the value at the given Index2D position.</summary>
        public virtual void UpdateTile (IBoardIndex position, T value) {
            SetTile (position, value);
        }

        public virtual void DeleteTile (IBoardIndex position) {
            SetTile (position, default (T));
        }

        public virtual void InsertTile (IBoardIndex position, T value) {
            if (AreEqual (GetTile (position), default (T))) {
                SetTile (position, value);
            }
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

        public virtual void SplitTile (IBoardIndex fromPosition, IBoardIndex toPosition, T fromValue, T toValue) {
            SetTile (toPosition, toValue);
            SetTile (fromPosition, fromValue);
        }

        /// <summary>Resets all MoveVectors.  Sets all values to default, using the current mergeStrategy.</summary>
        public virtual void Clear () {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    values[x,y] = default (T);
                }
            }
        }

        /// <summary>Insert, update, or delete each value in a list of Index2D positions.</summary> 
        public void UpdateTiles (List<IBoardIndex> positions, T value) {
            foreach (IBoardIndex position in positions) {
                UpdateTile (position, value);
            }
        }

        /// <summary>Returns the value at the given Index2D position.</summary>
        public virtual T GetTile (IBoardIndex position) {
            return values[position.X, position.Y];
        }

        public virtual bool IsPositionValue (IBoardIndex position, T value) {
            return AreEqual (GetTile (position), value);
        }

        /// <summary>Checks if the given Index2D is within the bounds of the PuzzleBoard</summary>
        public bool IsValidIndex2D (IBoardIndex index) {
            if (index.X < 0 || index.X >= width || index.Y < 0 || index.Y >= height) {
                return false;
            } else {
                return true;
            }
        }

        /// <summary>Sets the value at the given Index2D position.</summary>
        protected void SetTile (IBoardIndex position, T value) {
            if (!IsValidIndex2D (position)) {
                return;
            }
            values[position.X, position.Y] = value;
        }

        public static bool AreEqual (T valueA, T valueB) {
            return EqualityComparer<T>.Default.Equals (valueA, valueB);
        }

        public static bool IsEmpty (T value) {
            return AreEqual (value, default (T));
        }
    }

}
