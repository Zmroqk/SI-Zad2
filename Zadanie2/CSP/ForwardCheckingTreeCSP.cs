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
    internal class ForwardCheckingTreeCSP<T> : ICSPTree<T>
    {
        public long Iterations { get; private set; }
        public List<Variable<T>> Variables { get; }
        Func<List<T>, IHeuristic<T>> HeuristicFactoryDomain { get; }
        Func<List<Variable<T>>, IHeuristic<Variable<T>>> HeuristicFactoryChild { get; }
        List<Func<List<Variable<T>>, Variable<T>, IConstraint>> ConstraintsFactories { get; }
        List<IConstraint> Constraints { get; }
        Func<List<Variable<T>>, Variable<T>, IForwardCheck> ForwardFactory { get; }
        public List<List<Variable<T>>> Solutions { get; }

        public ForwardCheckingTreeCSP(
            List<Variable<T>> variables,
            List<IConstraint> constraints,
            Func<List<T>, IHeuristic<T>> heuristicFactoryDomain,
            Func<List<Variable<T>>, IHeuristic<Variable<T>>> heuristicFactoryChild,
            Func<List<Variable<T>>, Variable<T>, IForwardCheck> forwardFactory,
            List<Func<List<Variable<T>>, Variable<T>, IConstraint>> constraintsFactories)
        {
            Variables = variables;
            Constraints = constraints;
            HeuristicFactoryDomain = heuristicFactoryDomain;
            HeuristicFactoryChild = heuristicFactoryChild;
            ConstraintsFactories = constraintsFactories;
            ForwardFactory = forwardFactory;
            Iterations = 0;
            Solutions = new List<List<Variable<T>>>();
        }

        private bool CheckConstraints(Variable<T> current)
        {
            foreach (IConstraint constraint in Constraints)
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
            Dictionary<Variable<T>, List<T>> domains = Variables
                .Select((v) => (v, v.CurrentDomain.ToList()))
                .ToDictionary(el => el.v, el => el.Item2);
            bool forwardCheckFlag = ForwardFactory.Invoke(Variables, newRoot).ForwardDomainsCheck();
            if (!forwardCheckFlag)
            {
                foreach (var domain in domains)
                {
                    if(domain.Key != newRoot || !domain.Key.Visited)
                    {
                        domain.Key.CurrentDomain.Clear();
                        domain.Key.CurrentDomain.AddRange(domain.Value);
                    }
                }
                return;
            }          
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
                child.CurrentDomain.AddRange(domains[child]);
            }
            foreach (var domain in domains)
            {
                if (domain.Key != newRoot || !domain.Key.Visited)
                {
                    domain.Key.CurrentDomain.Clear();
                    domain.Key.CurrentDomain.AddRange(domain.Value);
                }
            }
            Iterations++;
        }

        private void FindSolutionInChild(Variable<T> newRoot)
        {
            newRoot.Visited = true;
            List<T> domain = newRoot.CurrentDomain.ToList();
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
            newRoot.CurrentDomain.Clear();
            newRoot.CurrentDomain.AddRange(domain);
            newRoot.Visited = false;
            return;
        }

        public bool FindSolution()
        {
            foreach (Variable<T> variable in Variables)
            {
                if(variable.IsConstant)
                    ForwardFactory.Invoke(Variables, variable).ForwardDomainsCheck();
            }
            Variable<T> root;
            root = HeuristicFactoryChild.Invoke(Variables).Evaluate();
            root.Visited = true;
            FindSolutionInChild(root);
            return Solutions.Count > 0;
        }
    }
}
