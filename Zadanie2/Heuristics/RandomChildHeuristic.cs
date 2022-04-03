using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Heuristics
{
    internal class RandomChildHeuristic<T> : IHeuristic<Variable<T>>
    {
        public List<Variable<T>> Variables { get; }

        public RandomChildHeuristic(List<Variable<T>> variables)
        {
            Variables = new List<Variable<T>>();
            for (int i = 0; i < variables.Count; i++)
            {
                if(!variables[i].Visited)
                    Variables.Add(variables[i]);
            }
        }

        public Variable<T>? Evaluate()
        {
            Random ran = new Random();
            return Variables.Count > 0 ? Variables[ran.Next(0, Variables.Count)] : null;
        }
    }
}
