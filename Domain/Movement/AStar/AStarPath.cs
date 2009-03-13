using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEnumerable=System.Collections.IEnumerable;

namespace LastManStanding.Domain.Movement.AStar
{
    public class Path<T> : IEnumerable<T>
    {
        public T LastStep { get; private set; }
        public Path<T> PreviousSteps { get; private set; }
        public decimal TotalCost { get; private set; }

        private Path(T lastStep, Path<T> previousSteps, decimal totalCost)
        {
            LastStep = lastStep;
            PreviousSteps = previousSteps;
            TotalCost = totalCost;
        }
        public Path(T start) : this(start, null, 0) { }

        public Path<T> AddStep(T step, decimal stepCost)
        {
            return new Path<T>(step, this, TotalCost + stepCost);
        }



        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            for (var p = this; p != null; p = p.PreviousSteps)
                yield return p.LastStep;
        }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
