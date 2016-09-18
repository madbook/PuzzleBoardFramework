namespace PuzzleBoardFramework {

    /// <summary>Static class that only provides default MergeStrategy instances to PuzzleBoard instances.</summary>
    public static class MergeStrategy {
        /// <summary>Returns a default MergeStrategy for the given type.</summary>
        public static IMergeStrategy<T> GetDefaultStrategy<T> () {
            if (typeof (T) == typeof (int))
                return new IntMergeStrategy () as IMergeStrategy<T>;
            else
                return new GenericMergeStrategy<T> () as IMergeStrategy<T>;
        }
    }

    /// <summary>A generic default MergeStrategy for any type.</summary>
    public class GenericMergeStrategy<T> : IMergeStrategy<T> {
        public virtual bool ShouldPush (T from, T into) {
            return !IsEmpty (into);
        }

        public virtual bool ShouldMerge (T from, T into) {
            return IsEmpty (into);
        }

        public virtual T Merge (T from, T into) {
            return from;
        }

        public virtual bool IsEmpty (T value) {
            return value.Equals (Empty ());
        }

        public virtual T Empty () {
            return default (T);
        }
    }

    /// <summary>A default MergeStrategy for integer values.</summary>
    public class IntMergeStrategy : IMergeStrategy<int> {
        public virtual bool ShouldPush (int from, int into) {
            return !IsEmpty (into);
        }

        public virtual bool ShouldMerge (int from, int into) {
            return from == into || IsEmpty (into);
        }

        public virtual int Merge (int from, int into) {
            return from + into;
        }

        public virtual bool IsEmpty (int value) {
            return value == 0;
        } 

        public virtual int Empty () {
            return 0;
        }
    }

}
