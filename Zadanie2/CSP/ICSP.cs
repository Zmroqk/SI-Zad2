using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.Components;

namespace Zadanie2.CSP
{
    internal interface ICSP<T>
    {
        List<Variable<T>[,]> Solutions { get; }
        long Iterations { get; }
        bool FindSolution();
    }
}
