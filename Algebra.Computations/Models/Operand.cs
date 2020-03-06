using System;
using System.Collections.Generic;

namespace Algebra.Computations.Models
{
    public class Operand : IEquatable<Operand>, IEqualityComparer<Operand>
    {
        public string Name { get; set; }
        public decimal? Value { get; set; }
        public OperandType Type { get; set; }
        public Operation Operation { get; set; }

        public bool Equals(Operand other)
        {
            if (other == null)
                return false;

            var result = Type == other.Type
                && Operation == other.Operation
                && Value == other.Value
                && Name == other.Name;

            return result;
        }

        public bool Equals(Operand x, Operand y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Operand obj)
        {
            return obj.GetHashCode();
        }

        public override string ToString()
        {
            var multiplier = Value == 1 ? "" : Value == -1 ? "-" : Value.ToString();
            if(Type == OperandType.Variable)
            {
                return $"{multiplier}{Name}";
            }
            else
            {
                return $"{multiplier}{Operation}";
            }
        }
    }
}