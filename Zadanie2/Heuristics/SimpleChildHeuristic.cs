using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Heuristics
{
    internal class SimpleChildHeuristic<T> : IHeuristic<Variable<T>>
    {
        public Variable<T>[,] Variables { get; }

        public SimpleChildHeuristic(int size, List<Variable<T>> variables)
        {
            Variables = new Variable<T>[size, size];
            int index = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Variables[i, j] = variables[index++];
                }
            }
        }

        public Variable<T>? Evaluate()
        {
            for (int i = 0; i < Variables.GetLength(0); i++)
            {
                for (int j = 0; j < Variables.GetLength(1); j++)
                {
                    if(Variables[i, j].Visited == false)
                    {
                        return Variables[i, j];
                    }
                }
            }
            return null;
        }
    }
}
