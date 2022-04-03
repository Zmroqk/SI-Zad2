using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Heuristics
{
    internal class SimpleHeuristic<T> : IHeuristic<T>
    {
        List<T> Domain { get; }

        public SimpleHeuristic(List<T> domain){
            Domain = domain;
        }

        public T Evaluate()
        {
            if (Domain.Count == 0)
                throw new ArgumentException("Domain cannot be empty");
            T result = Domain.First();
            Domain.RemoveAt(0);
            return result;
        }
    }
}
