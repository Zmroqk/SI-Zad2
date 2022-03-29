using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Components
{
    internal class Variable<T>
    {
        public Variable(T value, List<T> domain)
        {
            Value = value;
            Domain = domain;
            CurrentDomain = domain.ToList();
            IsConstant = true;
        }

        public Variable(List<T> domain)
        {
            Value = default(T);
            Domain = domain;
            CurrentDomain = domain.ToList();
            IsConstant = false;
        }

        public Variable(T value)
        {
            Value = value;
            IsConstant = true;
            Domain = new List<T>();
            CurrentDomain = new List<T>();
        }

        public Variable()
        {
            Value = default(T);
            Domain = new List<T>();
            CurrentDomain = new List<T>();
            IsConstant = false;
        }

        public Variable(Variable<T> other){
            Value = other.Value;
            Domain = other.Domain.ToList();
            CurrentDomain = other.CurrentDomain.ToList();
            IsConstant = other.IsConstant;
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
        public bool IsConstant { get; }
    }
}
