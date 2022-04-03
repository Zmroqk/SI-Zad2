using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Heuristics
{
    internal interface IHeuristic<T>
    {
        public T? Evaluate();
    }
}
