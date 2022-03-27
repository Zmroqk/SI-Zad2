using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Components
{
    internal class InequalityConstraint : IConstraint
    {
        Variable<int?> FirstVariable { get; }
        Variable<int?> SecondVariable { get; }

        /// Futoshiki constraint requires first variable to be bigger than second variable
        public InequalityConstraint(Variable<int?> firstVariable, Variable<int?> secondVariable)
        {
            FirstVariable = firstVariable;
            SecondVariable = secondVariable;
        }

        public bool CheckConstraint()
        {
            if (FirstVariable.Value.HasValue && SecondVariable.Value.HasValue)
                return FirstVariable.Value > SecondVariable.Value;
            else
                return true;
        }
    }
}
