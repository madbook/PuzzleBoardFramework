using System;
using System.Collections.Generic;

namespace PuzzleBoardFramework {

    public class Publisher<T> : IPublisher<T> {
        List<Action<T>> subscribers = new List<Action<T>> ();

        public void Subscribe (Action<T> subscriber) {
            subscribers.Add (subscriber);
        }

        public void Publish (T update) {
            foreach (Action<T> subscriber in subscribers) {
                subscriber (update);
            }
        }
    }

}