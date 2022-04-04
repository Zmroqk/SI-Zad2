using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.ForwardChecking
{
    internal class BinaryForwardCheckTree : IForwardCheck
    {
        Variable<short?>[,] Variables { get; }
        int X { get; }
        int Y { get; }
        short? Value { get; }

        public BinaryForwardCheckTree(int size, List<Variable<short?>> variables, Variable<short?> current)
        {
            Variables = new Variable<short?>[size, size];
            int index = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Variables[i, j] = variables[index++];
                    if (Variables[i, j] == current)
                    {
                        X = i;
                        Y = j;
                    }
                }
            }
            Value = current.Value;
        }

        public bool ForwardDomainsCheck()
        {
            if (X + 1 < Variables.GetLength(0) && X - 1 >= 0)
            {
                if (Variables[X - 1, Y].Value.HasValue && Variables[X, Y].Value.HasValue)
                    if (Variables[X, Y].Value == Variables[X - 1, Y].Value && X + 1 != X)
                        Variables[X + 1, Y].CurrentDomain.Remove(Value);
                if (Variables[X + 1, Y].CurrentDomain.Count == 0 && X + 1 != X && !Variables[X + 1, Y].IsConstant)
                    return false;
            }
            if (Y + 1 < Variables.GetLength(1) && Y - 1 >= 0)
            {
                if (Variables[X, Y - 1].Value.HasValue && Variables[X, Y].Value.HasValue)
                    if (Variables[X, Y].Value == Variables[X, Y - 1].Value && Y + 1 != Y)
                        Variables[X, Y + 1].CurrentDomain.Remove(Value);
                if (Variables[X, Y + 1].CurrentDomain.Count == 0 && Y + 1 != Y && !Variables[X, Y + 1].IsConstant)
                    return false;
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
