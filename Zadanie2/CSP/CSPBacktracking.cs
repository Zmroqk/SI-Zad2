using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.Constraints;
using Zadanie2.Heuristics;

namespace Zadanie2.CSP
{
    internal class CSPBacktracking<T> : ICSP<T>
    {
        public long Iterations { get; private set; }
        public Variable<T>[,] Variables { get; }
        Func<List<T>, IHeuristic<T>> HeuristicFactory { get; }
        List<Func<Variable<T>[,], int, int, IConstraint>> ConstraintsFactories { get; }
        List<IConstraint> Constraints { get; }
        public List<Variable<T>[,]> Solutions { get; }
        public CSPBacktracking(Variable<T>[,] variables,
            List<IConstraint> constraints,
            Func<List<T>, IHeuristic<T>> heuristicFactory,
            List<Func<Variable<T>[,], int, int, IConstraint>> constraintsFactories)
        {
            Variables = variables;
            Constraints = constraints;
            HeuristicFactory = heuristicFactory;
            ConstraintsFactories = constraintsFactories;
            Iterations = 0;
            Solutions = new List<Variable<T>[,]>();
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

        private (int, int) FindPreviousIndexesNotConstant(int i, int j)
        {
            do
            {
                if (!Variables[i, j].IsConstant)
                {
                    Variables[i, j].Value = default(T);
                    Variables[i, j].CurrentDomain.Clear();
                    Variables[i, j].CurrentDomain.AddRange(Variables[i, j].Domain);
                }
                (i, j) = FindPreviousIndexes(i, j);
            }
            while (i >= 0 && (Variables[i, j].IsConstant || Variables[i, j].CurrentDomain.Count == 0));
            return (i, j);
        }

        private bool CheckConstraints(int i, int j) 
        {
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
            if (checkSuccess)
            {
                foreach(var constraintFactory in ConstraintsFactories)
                {
                    bool check = constraintFactory.Invoke(Variables, i, j).CheckConstraint();
                    if (!check)
                    {
                        checkSuccess = false;
                        break;
                    }
                }
            }
            return checkSuccess;
        }

        private void AddCurrentToSolution()
        {
            Variable<T>[,] solution = new Variable<T>[Variables.GetLength(0), Variables.GetLength(1)];
            for (int k = 0; k < Variables.GetLength(0); k++)
            {
                for (int l = 0; l < Variables.GetLength(1); l++)
                {
                    solution[k, l] = new Variable<T>(Variables[k, l]);
                }
            }
            Solutions.Add(solution);
        }

        public bool FindSolution()
        {
            Solutions.Clear();
            int i = 0, j = 0;
            while(i >= 0)
            {
                if(Variables[i, j].IsConstant && i == Variables.GetLength(0) - 1 && j == Variables.GetLength(1) - 1)
                {
                    bool checkSuccess = CheckConstraints(i, j);                 
                    if (checkSuccess)
                    {
                        AddCurrentToSolution();
                    }
                    (i, j) = FindPreviousIndexesNotConstant(i, j);
                }
                else if(!Variables[i, j].IsConstant && Variables[i, j].CurrentDomain.Count == 0)
                {
                    (i, j) = FindPreviousIndexesNotConstant(i, j);
                }
                else if(!Variables[i, j].IsConstant)
                {
                    Variables[i, j].Value = HeuristicFactory.Invoke(Variables[i, j].CurrentDomain).Evaluate();
                    bool checkSuccess = CheckConstraints(i, j);
                    if (checkSuccess && i == Variables.GetLength(0) - 1 && j == Variables.GetLength(1) - 1)
                    {
                        AddCurrentToSolution();
                    }                   
                    else if(checkSuccess)
                        (i, j) = FindNextIndexes(i, j);
                }
                else
                    (i, j) = FindNextIndexes(i, j);
                Iterations++;
            }
            return Solutions.Count > 0;
        }
    }
}
