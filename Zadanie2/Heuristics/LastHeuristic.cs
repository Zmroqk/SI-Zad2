using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Heuristics
{
    internal class LastHeuristic<T> : IHeuristic<T>
    {
        List<T> Domain { get; }

        public LastHeuristic(List<T> domain){
            Domain = domain;
        }

        public T Evaluate()
        {
            if (Domain.Count == 0)
                throw new ArgumentException("Domain cannot be empty");
            T result = Domain.Last();
            Domain.RemoveAt(Domain.Count-1);
            return result;
        }
    }
}
