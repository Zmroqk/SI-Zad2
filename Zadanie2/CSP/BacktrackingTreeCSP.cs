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
    internal class BacktrackingTreeCSP<T> : ICSPTree<T>
    {
        public long Iterations { get; private set; }
        public List<Variable<T>> Variables { get; }
        Func<List<T>, IHeuristic<T>> HeuristicFactoryDomain { get; }
        Func<List<Variable<T>>, IHeuristic<Variable<T>>> HeuristicFactoryChild { get; }
        List<Func<List<Variable<T>>, Variable<T>, IConstraint>> ConstraintsFactories { get; }
        List<IConstraint> Constraints { get; }
        public List<List<Variable<T>>> Solutions { get; }

        public BacktrackingTreeCSP(
            List<Variable<T>> variables,
            List<IConstraint> constraints,
            Func<List<T>, IHeuristic<T>> heuristicFactoryDomain,
            Func<List<Variable<T>>, IHeuristic<Variable<T>>> heuristicFactoryChild,
            List<Func<List<Variable<T>>, Variable<T>, IConstraint>> constraintsFactories)
        {
            Variables = variables;
            Constraints = constraints;
            HeuristicFactoryDomain = heuristicFactoryDomain;
            HeuristicFactoryChild = heuristicFactoryChild;
            ConstraintsFactories = constraintsFactories;
            Iterations = 1;
            Solutions = new List<List<Variable<T>>>();
        }

        private bool CheckConstraints(Variable<T> current)
        {
            foreach(IConstraint constraint in Constraints)
            {
                if (!constraint.CheckConstraint())
                {
                    return false;
                }
            }
            foreach (var factory in ConstraintsFactories)
            {
                if (!factory.Invoke(Variables, current).CheckConstraint())
                {
                    return false;
                }
            }
            return true;
        }

        private void SaveSolution()
        {
            Solutions.Add(Variables.Select(v => new Variable<T>(v)).ToList());
        }

        private void SolutionFinderHelper(Variable<T> newRoot)
        {
            if (!newRoot.IsConstant)
                newRoot.Value = HeuristicFactoryDomain.Invoke(newRoot.CurrentDomain).Evaluate();
            bool successFlag = CheckConstraints(newRoot);
            if (successFlag)
            {
                Variable<T>? child = HeuristicFactoryChild.Invoke(Variables).Evaluate();
                if (child != null)
                    FindSolutionInChild(child);
                else
                {
                    SaveSolution();
                    return;
                }
                child.Value = default(T);
                child.CurrentDomain.Clear();
                child.CurrentDomain.AddRange(child.Domain);
            }
        }

        private void FindSolutionInChild(Variable<T> newRoot)
        {
            newRoot.Visited = true;
            if (newRoot.IsConstant)
            {
                SolutionFinderHelper(newRoot);
            }
            else
            {
                for (int k = 0; k < newRoot.CurrentDomain.Count;)
                {
                    SolutionFinderHelper(newRoot);
                }
            }
            newRoot.Visited = false;
            Iterations++;
            return;
        }

        public bool FindSolution()
        {
            Variable<T> root;
            root = HeuristicFactoryChild.Invoke(Variables).Evaluate();
            root.Visited = true;
            FindSolutionInChild(root);
            return Solutions.Count > 0;
        }
    }
}
