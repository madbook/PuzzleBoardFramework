
using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class History<T> : ITurnRecorder<T> {

        Stack<Stack<T>> history = new Stack<Stack<T>> ();
        Stack<T> currentTurn = new Stack<T> ();

        public int Count {
            get { return history.Count; }
        }

        public IEnumerable<T> IterateLastTurn () {
            Stack<T> lastTurn = history.Peek ();

            foreach (T record in lastTurn) {
                yield return record;
            }
        }

        public void ClearLastTurn () {
            Stack<T> lastTurn = history.Pop ();
            lastTurn.Clear ();
        }

        public void ClearAll () {
            foreach (Stack<T> turn in history) {
                turn.Clear ();
            }
            history.Clear ();
            currentTurn.Clear ();
        }


        public void AddRecord (T record) {
            currentTurn.Push (record);
        }

        public void NewTurn () {
            if (currentTurn.Count > 0) {
                history.Push (currentTurn);
            }
            currentTurn = new Stack<T> ();
        }
    }
}
