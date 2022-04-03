using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.Constraints;

namespace Zadanie2.ForwardChecking
{
    internal class FutoshikiForwardCheck : IForwardCheck
    {
        List<InequalityConstraint> Constraints { get; }
        Variable<int?>[,] Variables { get; }
        int X { get; }
        int Y { get; }
        int? Value { get; }


        public FutoshikiForwardCheck(
            Variable<int?>[,] variables, 
            List<InequalityConstraint> constraints, 
            int x, 
            int y,
            int? value)
        {
            Variables = variables;
            X = x;
            Y = y;
            Value = value;
            Constraints = constraints;
        }

        public bool ForwardDomainsCheck()
        {
            for(int i = 0; i < Variables.GetLength(0); i++)
            {
                if(i != X && !Variables[i, Y].Value.HasValue)
                {
                    Variables[i, Y].CurrentDomain.Remove(Value);
                    if (Variables[i, Y].CurrentDomain.Count == 0)
                    {
                        return false;
                    }
                }
                if(i != Y && !Variables[X, i].Value.HasValue)
                {
                    Variables[X, i].CurrentDomain.Remove(Value);
                    if (Variables[X, i].CurrentDomain.Count == 0)
                    {
                        return false;
                    }
                }
            }
            List<InequalityConstraint> involved = Constraints
                .Where(c => c.FirstVariable == Variables[X, Y] || c.SecondVariable == Variables[X, Y]).ToList();
            foreach(InequalityConstraint constraint in involved)
            {
                if(constraint.FirstVariable == Variables[X, Y] && constraint.FirstVariable.Value.HasValue && !constraint.SecondVariable.Value.HasValue)
                {
                    List<int?> removed = constraint.SecondVariable.CurrentDomain.Where((val) => val >= Value).ToList();
                    foreach(int? remove in removed)
                    {
                        constraint.SecondVariable.CurrentDomain.Remove(remove);
                    }
                    if (constraint.SecondVariable.CurrentDomain.Count == 0) {
                        return false;
                    }
                }
                if (constraint.SecondVariable == Variables[X, Y] && constraint.SecondVariable.Value.HasValue && !constraint.FirstVariable.Value.HasValue)
                {
                    List<int?> removed = constraint.FirstVariable.CurrentDomain.Where((val) => val <= Value).ToList();
                    foreach (int? remove in removed)
                    {
                        constraint.FirstVariable.CurrentDomain.Remove(remove);
                    }
                    if (constraint.FirstVariable.CurrentDomain.Count == 0) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
