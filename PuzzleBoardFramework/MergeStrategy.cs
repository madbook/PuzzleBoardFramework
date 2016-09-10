namespace PuzzleBoardFramework {

    /// <summary>Static class that only provides default MergeStrategy instances to PuzzleBoard instances.</summary>
    public static class MergeStrategy {
        /// <summary>Returns a default MergeStrategy for the given type.</summary>
        public static MergeStrategy<T> GetDefaultStrategy<T> () {
            if (typeof (T) == typeof (int))
                return new IntMergeStrategy () as MergeStrategy<T>;
            else
                return new GenericMergeStrategy<T> () as MergeStrategy<T>;
        }
    }

    /// <summary>A class that represent a strategy for dealing with cell interactions on the board for a given type.</summary>
    public abstract class MergeStrategy<T> {
        public abstract bool ShouldPush (T from, T into);
        public abstract bool ShouldMerge (T from, T into);
        public abstract T Merge (T from, T into);
        public abstract bool IsEmpty (T value);
        public abstract T Empty ();
    }

    /// <summary>A generic default MergeStrategy for any type.</summary>
    public class GenericMergeStrategy<T> : MergeStrategy<T> {
        public override bool ShouldPush (T from, T into) {
            return !IsEmpty (into);
        }

        public override bool ShouldMerge (T from, T into) {
            return IsEmpty (into);
        }

        public override T Merge (T from, T into) {
            return from;
        }

        public override bool IsEmpty (T value) {
            return value.Equals (Empty ());
        }

        public override T Empty () {
            return default (T);
        }
    }

    /// <summary>A default MergeStrategy for integer values.</summary>
    public class IntMergeStrategy : MergeStrategy<int> {
        public override bool ShouldPush (int from, int into) {
            return !IsEmpty (into);
        }

        public override bool ShouldMerge (int from, int into) {
            return from == into || IsEmpty (into);
        }

        public override int Merge (int from, int into) {
            return from + into;
        }

        public override bool IsEmpty (int value) {
            return value == 0;
        } 

        public override int Empty () {
            return 0;
        }
    }

}
