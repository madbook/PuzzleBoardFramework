
using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class HistoryManager<T> {

        Stack<Stack<Record<T>>> history;
        Stack<Record<T>> currentTurn;

        public HistoryManager () {
            history = new Stack<Stack<Record<T>>> ();
            currentTurn = new Stack<Record<T>> ();
        }

        public int Count {
            get { return history.Count; }
        }

        public IEnumerable<Record<T>> IterateLastTurn () {
            Stack<Record<T>> lastTurn = history.Peek ();

            foreach (Record<T> record in lastTurn) {
                yield return record;
            }
        }

        public void ClearLastTurn () {
            Stack<Record<T>> lastTurn = history.Pop ();
            lastTurn.Clear ();
        }

        public void ClearAll () {
            foreach (Stack<Record<T>> turn in history) {
                turn.Clear ();
            }
            history.Clear ();
            currentTurn.Clear ();
        }


        public void AddRecord (Record<T> record) {
            currentTurn.Push (record);
        }

        public void NewTurn () {
            if (currentTurn.Count > 0) {
                history.Push (currentTurn);
            }
            currentTurn = new Stack<Record<T>> ();
        }
    }

}
