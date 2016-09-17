using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBoardFramework {

    /// <summary>A 2D integer vector that represents a position on a board.</summary>
    public interface IBoardIndex {
        int X { get; }

        int Y { get; }
    }

    /// <summary>Represents a tile container that can be addressed and updated by IBoardIndex values.</summary>
    public interface IUpdatableBoard<T> {
        /// <summary>Update the tile at the given position with the given value.</summary>
        /// <remarks>
        ///     Implementations should defer to DeleteTile if default (T) is passed in.
        ///     Implementations should defer to InsertTile if the current value at position is default (T).
        ///     Implementations should do nothing is the current value and value to set are both default (T).
        /// </remarks>
        void UpdateTile (IBoardIndex position, T value);

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

}
