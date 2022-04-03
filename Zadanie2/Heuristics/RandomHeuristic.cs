using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Heuristics
{
    internal class RandomHeuristic<T> : IHeuristic<T>
    {
        List<T> Domain { get; }

        public RandomHeuristic(List<T> domain)
        {
            Domain = domain;
        }

        public T Evaluate()
        {
            Random random = new Random();
            if (Domain.Count == 0)
                throw new ArgumentException("Domain cannot be empty");
            int index = random.Next(0, Domain.Count);
            T result = Domain[index];
            Domain.RemoveAt(index);
            return result;
        }
    }
}
