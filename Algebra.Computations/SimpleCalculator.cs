using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Algebra.Computations
{
    public class SimpleCalculator : ICalculator
    {
        private char separator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

        public string Simplify(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var tokens = GetTokens(input);
            var simple = Simplify(tokens);
            var result = Stringify(simple);
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

        private class Token
        {
            public string Name { get; set; }
            public decimal Multiplier { get; set; }
        }

        private List<Token> GetTokens(string input)
        {
            input += ' ';
            var result = new List<Token>();

            var onNumeral = false;
            var onVar = false;
            var str = string.Empty;
            var multiplier = 1M;
            var equalityMultiplier = 1M;

            for (var i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]) || input[i] == separator)
                {
                    if (onVar)
                    {
                        str += input[i];
                    }
                    else
                    {
                        onNumeral = true;
                        str += input[i];
                    }
                }
                else if (char.IsLetter(input[i]) || input[i] == '^')
                {
                    if (onVar)
                    {
                        str += input[i];
                    }
                    else if (onNumeral)
                    {
                        if (!decimal.TryParse(str, out multiplier))
                        {
                            throw new ArgumentException();
                        }
                        str = string.Empty + input[i];
                        onNumeral = false;
                        onVar = true;
                    }
                    else
                    {
                        str = string.Empty + input[i];
                        onVar = true;
                    }
                }
                else if (char.IsWhiteSpace(input[i])
                    || input[i] == '+'
                    || input[i] == '-'
                    || input[i] == '=')
                {                 
                    if (str != string.Empty)
                    {
                        if (onNumeral)
                        {
                            if (!decimal.TryParse(str, out var num))
                            {
                                throw new ArgumentException();
                            }
                            multiplier *= num;
                            str = string.Empty;
                        }

                        result.Add(new Token() { Name = str, Multiplier = multiplier * equalityMultiplier });
                        str = string.Empty;
                        multiplier = input[i] == '-' ? -1M : 1M;
                        onVar = false;
                        onNumeral = false;
                    }
                    
                    if (input[i] == '=')
                    {
                        if (equalityMultiplier == -1)
                            throw new ArgumentException();

                        equalityMultiplier = -1M;
                    }

                }
                else
                {
                    throw new ArgumentException("Wrong character");
                }
            }

            return result;
        }

        private List<Token> Simplify(List<Token> tokens)
        {
            var result = tokens.GroupBy(x => x.Name)
                .Select(x => new Token{ Name = x.Key, Multiplier = x.Sum(g => g.Multiplier) })
                .Where(x => x.Multiplier != 0)
                .ToList();

            return result;
        }

        private string Stringify(List<Token> tokens)
        {
            var sb = new StringBuilder();
            var left = tokens.Where(t => !string.IsNullOrEmpty(t.Name)).ToList();
            var right = tokens.SingleOrDefault(x => string.IsNullOrEmpty(x.Name));
            foreach(var token in left)
            {
                var multiplier = token.Multiplier == 1 ? ""
                    : token.Multiplier == -1 ? "-"
                    : (sb.Length == 0 ? "" : "+") + token.Multiplier.ToString();
                sb.Append($"{multiplier}{token.Name}");
            }
            sb.Append("=");
            sb.Append(-right?.Multiplier ?? 0);
            return sb.ToString();
        }
    }
}