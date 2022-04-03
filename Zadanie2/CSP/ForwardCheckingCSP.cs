using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.Constraints;
using Zadanie2.Heuristics;
using Zadanie2.ForwardChecking;


namespace Zadanie2.CSP
{
    internal class ForwardCheckingCSP<T> : ICSP<T>
    {
        public long Iterations { get; private set; }
        public Variable<T>[,] Variables { get; }
        Func<List<T>, IHeuristic<T>> HeuristicFactory { get; }
        Func<int, int, T, IForwardCheck> ForwardCheckFactory { get; }
        List<Func<Variable<T>[,], int, int, IConstraint>> ConstraintsFactories { get; }
        List<IConstraint> Constraints { get; }
        public List<Variable<T>[,]> Solutions { get; }

        Dictionary<(int x, int y), (int push, int pops)> pushpops;

        public ForwardCheckingCSP(Variable<T>[,] variables,
            List<IConstraint> constraints,
            Func<List<T>, IHeuristic<T>> heuristicFactory,
            Func<int, int, T, IForwardCheck> forwardCheckFactory,
            List<Func<Variable<T>[,], int, int, IConstraint>> constraintsFactories)
        {
            Variables = variables;
            Constraints = constraints;
            HeuristicFactory = heuristicFactory;
            ConstraintsFactories = constraintsFactories;
            ForwardCheckFactory = forwardCheckFactory;
            Iterations = 0;
            Solutions = new List<Variable<T>[,]>();
            pushpops = new Dictionary<(int x, int y), (int, int)>();
        }

        private (int, int) FindNextIndexes(int i, int j)
        {
            j++;
            if (j == Variables.GetLength(1))
            {
                i++;
                j = 0;
            }
            PushSingleDomain(i, j);
            return (i, j);
        }

        private (int, int) FindPreviousIndexes(int i, int j)
        {
            //PopSingleDomain(i, j); // ADDED
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
                    PopSingleDomain(i, j);
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
                foreach (var constraintFactory in ConstraintsFactories)
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

        private void PushSingleDomain(int x, int y)
        {
            Variables[x, y].PreviousDomains.Push(Variables[x, y].CurrentDomain.ToArray());
            if (!pushpops.ContainsKey((x, y)))
                pushpops.Add((x, y), (1, 0));
            else
                pushpops[(x, y)] = (pushpops[(x, y)].push + 1, pushpops[(x, y)].pops);
        }   

        private void PopSingleDomain(int x, int y)
        {
            if (!pushpops.ContainsKey((x, y)))
                pushpops.Add((x, y), (0, 1));
            else
                pushpops[(x, y)] = (pushpops[(x, y)].push, pushpops[(x, y)].pops + 1);
            if (Variables[x, y].PreviousDomains.Count > 0)
            {              
                T[] popedDomain = Variables[x, y].PreviousDomains.Pop();
                Variables[x, y].CurrentDomain.Clear();
                foreach (T t in popedDomain)
                {
                    Variables[x, y].CurrentDomain.Add(t);
                }
            }
        }

        private void PushDomainsCopy(int x, int y)
        {
            for(int i = x; i < Variables.GetLength(0); i++)
            {
                for(int j = y; j < Variables.GetLength(1); j++)
                {
                    if(i != x || j != y)
                    {
                        PushSingleDomain(i, j);
                    }
                }
            }
        }

        private void RestoreDomains(int x, int y)
        {
            for (int i = x; i < Variables.GetLength(0); i++)
            {
                for (int j = y; j < Variables.GetLength(1); j++)
                {
                    if (i != x || j != y)
                    {
                        PopSingleDomain(i, j);
                    }
                }
            }
        }
        private bool RemoveConstantConstraintsFromDomains()
        {
            for (int k = 0; k < Variables.GetLength(0); k++)
            {
                for (int l = 0; l < Variables.GetLength(1); l++)
                {
                    if (Variables[k, l].IsConstant)
                    {
                        bool forwardSuccess = ForwardCheckFactory.Invoke(k, l, Variables[k, l].Value).ForwardDomainsCheck();
                        if (!forwardSuccess)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        public bool FindSolution()
        {
            Solutions.Clear();
            if (!RemoveConstantConstraintsFromDomains())
            {
                return false;
            }
            int i = 0, j = 0;
            while (i >= 0)
            {
                //for (int k = 0; k < Variables.GetLength(0); k++)
                //{
                //    for (int l = 0; l < Variables.GetLength(1); l++)
                //    {
                //        Console.Write($"{(Variables[k, l].Value != null ? Variables[k, l].Value.ToString() : "x")}|");
                //    }
                //    Console.WriteLine();
                //}
                //Console.WriteLine();
                if (Variables[i,j].IsConstant)
                {
                    PushDomainsCopy(i, j);
                    bool forwardCheckSuccess = ForwardCheckFactory.Invoke(i, j, Variables[i, j].Value).ForwardDomainsCheck();
                    if (forwardCheckSuccess)
                    {
                        (i, j) = FindNextIndexes(i, j);
                    }
                    else
                    {
                        RestoreDomains(i, j);
                        FindPreviousIndexes(i, j);
                    }
                }
                if (Variables[i, j].IsConstant && i == Variables.GetLength(0) - 1 && j == Variables.GetLength(1) - 1)
                {
                    bool checkSuccess = CheckConstraints(i, j);
                    if (checkSuccess)
                    {
                        AddCurrentToSolution();
                    }
                    //(i, j) = FindPreviousIndexesNotConstant(i, j);
                    (i, j) = FindPreviousIndexes(i, j);
                }
                else if (!Variables[i, j].IsConstant && Variables[i, j].CurrentDomain.Count == 0)
                {
                    //(i, j) = FindPreviousIndexesNotConstant(i, j);
                    (i, j) = FindPreviousIndexes(i, j);
                }
                else if (!Variables[i, j].IsConstant)
                {
                    //RestoreDomains(i, j);
                    Variables[i, j].Value = HeuristicFactory.Invoke(Variables[i, j].CurrentDomain).Evaluate();
                    PushDomainsCopy(i, j);
                    bool forwardCheckSuccess = ForwardCheckFactory.Invoke(i, j, Variables[i, j].Value).ForwardDomainsCheck();
                    if (forwardCheckSuccess)
                    {
                        bool checkSuccess = CheckConstraints(i, j);
                        if (checkSuccess && i == Variables.GetLength(0) - 1 && j == Variables.GetLength(1) - 1)
                        {
                            AddCurrentToSolution();
                        }
                        else if (checkSuccess)
                            (i, j) = FindNextIndexes(i, j);
                        else
                            RestoreDomains(i, j);
                    }
                    else
                        RestoreDomains(i, j);
                }
                else
                    (i, j) = FindNextIndexes(i, j);
                Iterations++;
            }
            foreach(var e in pushpops){
                Console.WriteLine($"({e.Key.x},{e.Key.y}) pushed: {e.Value.push} popped: {e.Value.pops}");
            }
            return Solutions.Count > 0;
        }
    }
}
