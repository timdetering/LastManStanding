using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain
{
    public class PriorityQueue<P, V> : IEnumerable<KeyValuePair<P, V>>
    {
        private readonly SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();

        public bool IsEmpty
        {
            get { return !list.Any(); }
        }

        public void Enqueue(P priority, V value)
        {
            Queue<V> q;
            if (!list.TryGetValue(priority, out q))
            {
                q = new Queue<V>();
                list.Add(priority, q);
            }
            q.Enqueue(value);
        }

        public KeyValuePair<P, V> Dequeue()
        {
            // will throw if there isn’t any first element!
            var pair = list.First();
            var v = pair.Value.Dequeue();
            if (pair.Value.Count == 0) // nothing left of the top priority.
                list.Remove(pair.Key);

            return new KeyValuePair<P, V>(pair.Key, v);
        }

        public KeyValuePair<P, V> Peek()
        {
            var first = list.FirstOrDefault();

            if (first.Value != null)
                return new KeyValuePair<P, V>(first.Key, first.Value.Peek());

            return default(KeyValuePair<P, V>);
        }

        public int Count
        {
            get
            {
                return list.Sum(x => x.Value.Count);
            }
        }

        public IEnumerator<KeyValuePair<P, V>> GetEnumerator()
        {
            foreach (var keyValuePair in list)
                foreach (var v in keyValuePair.Value)
                    yield return new KeyValuePair<P, V>(keyValuePair.Key, v);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
