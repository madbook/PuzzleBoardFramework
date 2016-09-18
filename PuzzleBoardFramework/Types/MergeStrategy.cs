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
            return !BaseBoard<T>.IsEmpty (into);
        }

        public virtual bool ShouldMerge (T from, T into) {
            return BaseBoard<T>.IsEmpty (into);
        }

        public virtual T Merge (T from, T into) {
            return from;
        }
    }

    /// <summary>A default MergeStrategy for integer values.</summary>
    public class IntMergeStrategy : GenericMergeStrategy<int> {
        public override bool ShouldMerge (int from, int into) {
            return base.ShouldMerge (from, into) || from == into;
        }

        public override int Merge (int from, int into) {
            return from + into;
        }
    }

}
