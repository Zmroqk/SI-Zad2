using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.ForwardChecking
{
    internal class BinaryForwardCheck : IForwardCheck
    {
        Variable<short?>[,] Variables { get; }
        int X { get; }
        int Y { get; }
        short? Value { get; }

        public BinaryForwardCheck(Variable<short?>[,] variables, 
            int x,
            int y,
            short? value)
        {
            Variables = variables;
            X = x;
            Y = y;
            Value = value;
        }

        public bool ForwardDomainsCheck()
        {
            for(int i = X; i < Variables.GetLength(0) - 1; i++)
            {
                if (i + 1 < Variables.GetLength(0) && i - 1 >= 0)
                {
                    if(Variables[i-1, Y].Value.HasValue && Variables[i, Y].Value.HasValue)
                        if(Variables[i, Y].Value == Variables[i-1, Y].Value && i + 1 != X)
                            Variables[i+1, Y].CurrentDomain.Remove(Value);
                    if (Variables[i + 1, Y].CurrentDomain.Count == 0 && i + 1 != X && !Variables[i + 1, Y].IsConstant)
                        return false;
                }
            }
            for (int i = Y; i < Variables.GetLength(1) - 1; i++)
            {
                if (i + 1 < Variables.GetLength(1) && i - 1 >= 0)
                {
                    if (Variables[X, i - 1].Value.HasValue && Variables[X, i].Value.HasValue)
                        if (Variables[X, i].Value == Variables[X, i - 1].Value && i + 1 != Y)
                            Variables[X, i + 1].CurrentDomain.Remove(Value);
                    if (Variables[X, i + 1].CurrentDomain.Count == 0 && i + 1 != Y && !Variables[X, i + 1].IsConstant)
                        return false;
                }
            }
            if (X == Variables.GetLength(0) - 2)
            {
                int zeroes = 0, ones = 0;
                for (int i = 0; i <= X; i++)
                {
                    if(Variables[i, Y].Value == 0)
                        zeroes++;
                    if (Variables[i, Y].Value == 1)
                        ones++;
                }
                if (zeroes + ones == X)
                {
                    if (zeroes < ones)
                        Variables[X + 1, Y].CurrentDomain.Remove(1);
                    else
                        Variables[X + 1, Y].CurrentDomain.Remove(0);
                    if (Variables[X + 1, Y].CurrentDomain.Count == 0)
                        return false;
                }
            }
            if (Y == Variables.GetLength(1) - 2)
            {
                int zeroes = 0, ones = 0;
                for (int i = 0; i <= Y; i++)
                {
                    if (Variables[X, i].Value == 0)
                        zeroes++;
                    if (Variables[X, i].Value == 1)
                        ones++;
                }
                if (zeroes + ones == Y)
                {
                    if (zeroes < ones)
                        Variables[X, Y + 1].CurrentDomain.Remove(1);
                    else
                        Variables[X, Y + 1].CurrentDomain.Remove(0);
                    if (Variables[X, Y + 1].CurrentDomain.Count == 0)
                        return false;
                }
            }
            return true;
        }
    }
}
