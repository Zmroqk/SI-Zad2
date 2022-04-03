using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.Constraints;

namespace Zadanie2.CSP
{
    internal interface ICSPTree<T>
    {
        List<List<Variable<T>>> Solutions { get; }
        long Iterations { get; }
        bool FindSolution();
    }
}
