using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Components
{
    internal class FutoshikiConstraint : IConstraint
    {
        Variable<int?>[,] Variables { get; } 
        public FutoshikiConstraint(Variable<int?>[,] variables)
        {
            Variables = variables;
        }
        public bool CheckConstraint()
        {
            //Column and rows 3 same values
            for(int i = 0; i < Variables.GetLength(0); i++)
            {
                for(int j = 0; j < Variables.GetLength(1); j++)
                {
                    if(Variables[i, j].Value.HasValue)
                    {
                        int? value = Variables[i, j].Value;
                        for(int k = 0; k < Variables.GetLength(0); k++)
                        {
                            if(i != k && Variables[k, j].Value.HasValue)
                            {
                                if (value == Variables[k, j].Value)
                                    return false;
                            }
                        }
                        for (int k = 0; k < Variables.GetLength(1); k++)
                        {
                            if (j != k && Variables[i, k].Value.HasValue)
                            {
                                if (value == Variables[i, k].Value)
                                    return false;
                            }
                        }
                    }                 
                }
            }
            return true;
        }
    }
}
