using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2
{
    internal class Variable<T>
    {
        public Variable(T value, List<T> domain)
        {
            Value = value;
            Domain = domain;
            CurrentDomain = new List<T>();
            IsConstant = true;
            PreviousDomains = new Stack<T[]>();
            Visited = false;
        }

        public Variable(List<T> domain)
        {
            Value = default(T);
            Domain = domain;
            CurrentDomain = domain.ToList();
            IsConstant = false;
            PreviousDomains = new Stack<T[]>();
            Visited = false;
        }

        public Variable(T value)
        {
            Value = value;
            IsConstant = true;
            Domain = new List<T>();
            CurrentDomain = new List<T>();
            PreviousDomains = new Stack<T[]>();
            Visited = false;
        }

        public Variable()
        {
            Value = default(T);
            Domain = new List<T>();
            CurrentDomain = new List<T>();
            IsConstant = false;
            PreviousDomains = new Stack<T[]>();
            Visited = false;
        }

        public Variable(Variable<T> other){
            Value = other.Value;
            Domain = other.Domain.ToList();
            CurrentDomain = other.CurrentDomain.ToList();
            IsConstant = other.IsConstant;
            PreviousDomains = new Stack<T[]>();
            Visited = false;
        }

        private T? _value;

        public T? Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!IsConstant)
                    _value = value;
            }
        }
        public List<T> Domain { get; }
        public List<T> CurrentDomain { get; }
        public Stack<T[]> PreviousDomains { get; set; }
        public bool IsConstant { get; }
        public bool Visited { get; set; }
    }
}
