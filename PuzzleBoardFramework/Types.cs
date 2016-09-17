using System;

namespace PuzzleBoardFramework {

    public interface IBoardIndex {
        int X { get; }
        int Y { get; }
    }

    /// <summary>A 2D integer vector.</summary>
    public struct BoardPosition : IBoardIndex {
        readonly int x;
        readonly int y;

        public int X {
            get { return x; }
        }

        public int Y {
            get { return y; }
        }

        public BoardPosition (int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static BoardPosition operator + (BoardPosition position, MoveVector move) {
            return new BoardPosition (position.X + move.x, position.Y + move.y);
        }
    }

    public struct BoardState<T> : IBoardIndex {
        readonly int x;
        readonly int y;
        readonly T value;

        public int X {
            get { return x; }
        }

        public int Y {
            get { return y; }
        }

        public T Value {
            get { return value; }
        }

        public BoardState (int x, int y, T value) {
            this.x = x;
            this.y = y;
            this.value = value;
        }
    }

    /// <summary>A 2D integer normal vector.</summary>
    public struct MoveVector {
        public readonly int x;
        public readonly int y;

        public static MoveVector left = new MoveVector (-1, 0);
        public static MoveVector right = new MoveVector (1, 0);
        public static MoveVector down = new MoveVector (0, -1);
        public static MoveVector up = new MoveVector (0, 1);
        public static MoveVector zero = new MoveVector ();

        public MoveVector (int x, int y) {
            this.x = Math.Max (-1, Math.Min(x, 1));
            this.y = (this.x == 0) ? Math.Max (-1, Math.Min (y, 1)) : 0;
        }

        public static bool operator == (MoveVector a, MoveVector b) {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator != (MoveVector a, MoveVector b) {
            return !(a == b);
        }

        public override bool Equals (object a) {
            return this == (MoveVector)a;
        }

        public override int GetHashCode () {
            return (x + 1) + ((y + 1) << 2);
        }
    }

    /// <summary>A type of state change.</summary>
    public enum RecordType {
        Merge,
        Split,
        Move,
        Insert,
        Delete,
        Update,
    }

    public static class Record {
        public static RecordType GetOppositeRecordType (RecordType type) {
            if (type == RecordType.Split) {
                return RecordType.Merge;
            } else if (type == RecordType.Merge) {
                return RecordType.Split;
            } else if (type == RecordType.Insert) {
                return RecordType.Delete;
            } else if (type == RecordType.Delete) {
                return RecordType.Insert;
            } else {
                return type;
            }
        }
    }

    /// <summary>Represents a change of state on the board.</summary>
    public struct Record<T> {
        public readonly RecordType type;
        public readonly BoardPosition oldPosition;
        public readonly BoardPosition newPosition;
        public readonly T oldValue;
        public readonly T newValue;

        public Record (RecordType type, BoardPosition oldPosition, BoardPosition newPosition, T oldValue, T newValue) {
            this.type = type;   
            this.oldPosition = oldPosition;
            this.newPosition = newPosition;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

    }

}
