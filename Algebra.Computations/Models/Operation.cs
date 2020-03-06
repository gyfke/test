using System;
using System.Collections.Generic;
using System.Linq;

namespace Algebra.Computations.Models
{
    public class Operation : IEquatable<Operation>
    {
        public List<Operand> Operands { get; set; }
        public OperationType OperationType { get; set; }

        public bool Equals(Operation other)
        {
            if (other == null)
                return false;

            return OperationType == other.OperationType
                && Operands.SequenceEqual(other.Operands);
        }

        public override string ToString()
        {
            var result = string.Empty;
            foreach (var operand in Operands)
            {
                if (result != string.Empty)
                    result += OperationType == OperationType.Multiply ? "" :
                        (OperationType == OperationType.Add && operand.Value < 0) ?
                        "" : OperationType.Value;

                result += operand.ToString();
            }
            
            return result;
        }
    }
}
