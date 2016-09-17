using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBoardFramework {

    /// <summary>A 2D integer vector that represents a position on a board.</summary>
    public interface IBoardIndex {
        int X { get; }

        int Y { get; }
    }

    public interface IBoard {
        int Width { get; }

        int Height { get; }

        bool IsValidIndex2D (IBoardIndex index);
    }

    public interface IBoard<T> : IBoard {
        /// <summary>Get the value at the given position.</summary>
        T GetTile (IBoardIndex position);

        /// <summary>Checks the value at the current position to the given value for equality.</summary>
        bool IsPositionValue (IBoardIndex position, T value);
    }

    /// <summary>Represents a tile container that can be addressed and updated by IBoardIndex values.</summary>
    public interface IUpdatableBoard<T> : IBoard<T> {
        /// <summary>Update the tile at the given position with the given value.</summary>
        /// <remarks>
        ///     Implementations should defer to DeleteTile if default (T) is passed in.
        ///     Implementations should defer to InsertTile if the current value at position is default (T).
        ///     Implementations should do nothing is the current value and value to set are both default (T).
        /// </remarks>
        void UpdateTile (IBoardIndex position, T value);

        /// <summary>Update each tile in the given list of positions with the given value.</summary>
        /// <remarks>
        ///     Implementations should call either InsertTile, DeleteTile, or UpdateTile for each position.
        /// </remarks>
        void UpdateTiles (List<IBoardIndex> positions, T value);

        /// <summary>Set the value at position to default (T).</summary>
        void DeleteTile (IBoardIndex position);

        /// <summary>Set the value at position with value of default (T) to the given value.</summary>
        /// <remarks>
        ///     Implementations should do nothing if the value at position is not default (T).
        /// </remarks>
        void InsertTile (IBoardIndex position, T value);

        /// <summary>Set the value of toPosition to the current value at fromPosition and delete the value at fromPosition.</summary>
        /// <remarks>
        ///     Implementations should do nothing if the value at toPosition is not default (T).
        /// </remarks>
        void MoveTile (IBoardIndex fromPosition, IBoardIndex toPosition);

        /// <summary>Set the value at toPosition to the given value and delete the value at fromPosition.</summary>
        /// <remarks>
        ///     Implementations should do nothing if the value at either position is default (T).
        /// </remarks>
        void MergeTile (IBoardIndex fromPosition, IBoardIndex toPosition, T value);

        /// <summary>Set the value at fromPosition to fromValue and the value at toPosition to toValue.</summary>
        /// <remarks>
        ///     Implementations should do nothing if the value at toPosition is not default (T).
        ///     Implementations should do nothing if the value at fromPosition is default (T).
        /// </remarks>
        void SplitTile (IBoardIndex fromPosition, IBoardIndex toPosition, T fromValue, T toValue);

        /// <summary>Set the value at all positions to default (T).</summary>
        void Clear ();
    }

    /// <summary>Represents a tile container with values that can be moved around by applying MovementVectors.</summary>
    public interface IPushableBoard {
        /// <summary>Set the MoveVector at all positions to the given value.</summary>
        void PushAll (MoveVector push);

        /// <summary>Set the MoveVector at position to the given value.</summary>
        void PushTile (IBoardIndex position, MoveVector push);

        /// <summary>Set the MoveVector at each position to the given value.</summary> 
        void PushTiles (List<IBoardIndex> positions, MoveVector push);

        /// <summary>Perform all possible movements and merges based on the currently set movement vectors.</summary>
        void ApplyMoveVectors ();

        /// <summary>Perform movements and merges in the specified direction based on the currently set movement vectors.</summary>
        void ApplyMoveVectors (MoveVector move);
    }

    /// <summary>An IPushableBoard interface with additional type-specific methods.</summary>
    public interface IPushableBoard<T> : IPushableBoard {
        /// <summary>Set the MoveVector at all posisions matching the given value.</summary>
        void PushAllMatching (MoveVector move, T matchValue);
    }

    /// <summary>Provides methods to get lists of positions on a tile container.</summary>
    public interface ISearchableBoard<T> {
        /// <summary>Get a list of positions that match the given value.</summary>
        List<IBoardIndex> GetPositionsMatching (T matchValue);

        /// <summary>Get a list of positions that match any of the given values.</summary>
        List<IBoardIndex> GetPositionsMatching (params T[] valuesToMatch);

        /// <summary>Get a list of all positions in the given row.</summary>
        List<IBoardIndex> GetPositionsInRow (int row);

        /// <summary>Get a list of all positions in the given column.</summary>
        List<IBoardIndex> GetPositionsInColumn (int col);

        /// <summary>Get a list of all positions in the given row that match the given value.</summary>
        List<IBoardIndex> GetPositionsInRowMatching (int row, T matchValue);

        /// <summary>Get a list of all positions in the given column that match the given value.</summary>
        List<IBoardIndex> GetPositionsInColumnMatching (int col, T matchValue);

        /// <summary>Get a list of contiguous orthagonally adjacent tiles connected to the tile at the given position that match its value.</summary>
        List<IBoardIndex> GetIdenticalAdjacentPositions (T value, IBoardIndex position);
    }

    /// <summary>Represents an object that can render a tile container as GameObjects.</summary> 
    public interface IBoardRenderer<T> {
        /// <summary>Update obj rendering based on position.</summary>
        void UpdateRenderPosition (GameObject obj, IBoardIndex position, int z = 0);

        /// <summary>Update obj rendering based on value.</summary>
        void UpdateRenderValue (GameObject obj, T value);

        /// <summary>Update obj rendering based on value and movement direction.</summary>
        void UpdateRenderRotation (GameObject obj, T value, MoveVector move);
    }

    /// <summary>Represents an object that is animated.</summary>
    public interface IAnimatable {
        /// <summary>Determines whether or not animations should apply.</summary>
        bool Animating { get; set; }
    }

    /// <summary>Represents something that can record groups of values as turns.</summary>
    public interface ITurnRecorder<T> {
        /// <summary>The current number of recorded turns.</summary>
        /// <remarks>
        ///     The "current turn" that recorded values are appended to should not be counted.
        /// </remarks> 
        int Count { get; }

        /// <summary>Iterate over the the recorded values in the previous turn.</summary>
        /// <remarks>
        ///     Implementations should yield the recorded turns in the reverse order of their recording.
        /// </remarks>
        IEnumerable<T> IterateLastTurn ();

        /// <summary>Add the value to the current turn.</summary>
        void AddRecord (T value);

        /// <summary>Remove the most recently recorded turn and clear all values from it.</summary>
        void ClearLastTurn ();

        /// <summary>Remove all turns and all recorded values and clear the current turn.</summary>
        void ClearAll ();

        /// <summary>Save the current turn.</summary>
        void NewTurn ();
    }

    /// <summary>Provides methods for publishing and subscribing to updates.</summary>
    public interface IPublisher<T> {
        /// <summary>Add a callback action that will be called with each published update.</summary>
        void Subscribe (Action<T> subscriber);

        /// <summary>Publish an update that will be passed to all subscribers.</summary>
        void Publish (T update);

        /// TODO – this doesn't seem like the best place for this.
        /// <summary>Should revert a previously published record.</summary>
        void UndoRecord (T record);
    }

    /// <summary>Provides methods for determining how tiles should interact when pushed around.</summary>
    public interface IMergeStrategy<T> {
        /// <summary>Determines if the from value should push the into value.</summary>
        bool ShouldPush (T from, T into);

        /// <summary>Determines if the from value should merge with the into value.</summary>
        bool ShouldMerge (T from, T into);

        /// <summary>Provides a new value when the two given values merge.</summary>
        T Merge (T from, T into);

        /// <summary>Deprecated: Returns if the given value should be considered empty.</summary> 
        bool IsEmpty (T value);

        /// <summary>Deprecated: Returns a new empty value.</summary>
        T Empty ();
    }

}
