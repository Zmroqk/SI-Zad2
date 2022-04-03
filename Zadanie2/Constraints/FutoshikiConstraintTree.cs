using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.CSP;

namespace Zadanie2.Constraints
{
    internal class FutoshikiConstraintTree : IConstraint
    {
        Variable<int?>[,] Variables { get; } 
        int I { get; }
        int J { get; }

        public FutoshikiConstraintTree(int size, List<Variable<int?>> variables, Variable<int?> current)
        {
            Variables = new Variable<int?>[size, size];
            int index = 0;
            for(int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    Variables[i, j] = variables[index++];
                    if (Variables[i, j] == current)
                    {
                        I = i;
                        J = j;
                    }
                }
            }
        }
        public bool CheckConstraint()
        {
            //Column and rows 3 same values
            if(Variables[I, J].Value.HasValue)
            {
                int? value = Variables[I, J].Value;
                for(int k = 0; k < Variables.GetLength(0); k++)
                {
                    if(I != k && Variables[k, J].Value.HasValue)
                    {
                        if (value == Variables[k, J].Value)
                            return false;
                    }
                    if (J != k && Variables[I, k].Value.HasValue)
                    {
                        if (value == Variables[I, k].Value)
                            return false;
                    }
                }
            }                 
            return true;
        }
    }
}
