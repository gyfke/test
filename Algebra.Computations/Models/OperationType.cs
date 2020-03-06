using System;

namespace Algebra.Computations.Models
{
    public class OperationType : Enumeration<string>
    {
        public Func<decimal, decimal, decimal> Operation { get; set; }

        public OperationType(
            string value,
            string name,
            Func<decimal, decimal, decimal> operation) : base(value, name)
        {
            Operation = operation;
        }

        public static OperationType Add = new OperationType(
            "+",
            "Add",
            (x, y) => x + y);

        public static OperationType Substract = new OperationType(
            "-",
            "Substract",
            (x, y) => x - y);

        public static OperationType Multiply = new OperationType(
            "*",
            "Multiply",
            (x, y) => x * y);

        public static OperationType Divide = new OperationType(
            "/",
            "Add",
            (x, y) => x / y);

        public static OperationType Power = new OperationType(
            "^",
            "Power",
            (x, y) => Convert.ToDecimal(Math.Pow(Convert.ToDouble(x), Convert.ToDouble(y))));

    }
}