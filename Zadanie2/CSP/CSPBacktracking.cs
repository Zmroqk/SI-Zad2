using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.Components;

namespace Zadanie2.CSP
{
    internal class CSPBacktracking<T> : ICSP
    {
        public long Iterations { get; private set; }
        public Variable<T>[,] Variables { get; }
        List<IConstraint> Constraints { get; }
        public CSPBacktracking(Variable<T>[,] variables, List<IConstraint> constraints)
        {
            Variables = variables;
            Constraints = constraints;
            Iterations = 0;
        }

        private (int, int) FindNextIndexes(int i, int j)
        {
            j++;
            if (j == Variables.GetLength(1))
            {
                i++;
                j = 0;
            }
            return (i, j);
        }

        private (int, int) FindPreviousIndexes(int i, int j)
        {
            j--;
            if (j < 0)
            {
                i--;
                j = Variables.GetLength(1) - 1;
            }
            return (i, j);
        }

        public bool FindSolution()
        {
            int i = 0, j = 0;
            while(i < Variables.GetLength(0) && j < Variables.GetLength(1))
            {
                if(!Variables[i, j].IsConstant)
                {
                    Variables[i, j].Value = Variables[i, j].CurrentDomain[0];
                    Variables[i, j].CurrentDomain.RemoveAt(0);
                    bool checkSuccess = true;
                    foreach (var constraint in Constraints)
                    {
                        bool check = constraint.CheckConstraint();
                        if (!check)
                        {
                            checkSuccess = false;
                            break;
                        }
                    }
                    if (!checkSuccess && Variables[i, j].CurrentDomain.Count == 0)
                    {
                        do
                        {
                            if(!Variables[i, j].IsConstant)
                            {
                                Variables[i, j].Value = default(T);
                                Variables[i, j].CurrentDomain.Clear();
                                Variables[i, j].CurrentDomain.AddRange(Variables[i, j].Domain);
                            }
                            (i, j) = FindPreviousIndexes(i, j);
                            if (i < 0)
                                return false;
                        }
                        while (Variables[i, j].IsConstant || Variables[i, j].CurrentDomain.Count == 0);                          
                    }
                    else if(checkSuccess)
                        (i, j) = FindNextIndexes(i, j);
                }
                else
                    (i, j) = FindNextIndexes(i, j);
                Iterations++;
            }
            return true;
        }
    }
}
