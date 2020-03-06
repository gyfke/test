using System.Threading;
using System.Threading.Tasks;

namespace Algebra.Computations
{
    public class Calculator : ICalculator
    {
        private readonly IParser parser;

        public Calculator(IParser parser)
        {
            this.parser = parser;
        }

        public string Simplify(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var tokens = parser.GetTokens(input);
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);
            var simple = parser.Simplify(operands[0]);
            var result = simple.ToString();

            return result;
        }

        public async Task<string[]> SimplifyAsync(string[] input)
        {
            var result = new string[input.Length];
            await Task.Run(() =>
                Parallel.For(0, input.Length, (i, state) => {
                    result[i] = Simplify(input[i]);
                })
            );

            return result;
        }
    }
}