using System.Collections.Generic;
using Algebra.Computations.Models;

namespace Algebra.Computations
{
    public interface IParser
    {
        (Operand left, Operand right) Parse(string input);

        string[] GetTokens(string input);

        Queue<string> GetRPN(string[] tokens);

        Operand[] GetExpression(Queue<string> rpnQueue);

        Operand Simplify(Operand input);
    }
}