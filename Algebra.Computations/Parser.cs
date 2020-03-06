using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Algebra.Computations.Models;

namespace Algebra.Computations
{
    public class Parser : IParser
    {
        private string[] operators = { "+", "-", "*", "/", "^", "(", ")" };
        private char separator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

        private readonly Dictionary<string, int> OperatorPrecedences = 
            new Dictionary<string, int>()
            {
                { "+", 1},
                { "-", 1},
                { "*", 2},
                { "/", 2},
                { "^", 3},
                { "(", 0},
                { ")", 0}
            };
        
        public (Operand left, Operand right) Parse(string input)
        {
            var expressions = input.Split('=');

            // only simple expression (a+b) and equality expressions (a+b=c+d) are supported
            if (expressions.Length > 2)
            {
                throw new ArgumentException();
            }

            return (null, null);
        }

        public string[] GetTokens(string input)
        {
            List<string> result = new List<string>();

            var onNumeral = false;
            var onVar = false;
            var str = string.Empty;

            for (var i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]) || input[i] == separator)
                {
                    if (onVar)
                    {
                        throw new ArgumentException(); // x123 - not valid expression
                    }
                    else
                    {
                        onNumeral = true;
                        str += input[i];
                    }
                }
                else if (char.IsLetter(input[i]))
                {
                    if (onNumeral || onVar)
                    {
                        result.Add(str);
                        result.Add("*"); // Add operation explicitly ( 123x == 123*x)
                        str = string.Empty + input[i];
                        onNumeral = false;
                        onVar = true;
                    }
                    else
                    {
                        str += input[i];
                        onVar = true;
                    }
                }
                else if (char.IsWhiteSpace(input[i]))
                {
                    if (str != string.Empty)
                    {
                        result.Add(str);
                        str = string.Empty;
                        onVar = false;
                        onNumeral = false;
                    }
                }
                else if (operators.Contains(input[i].ToString()))
                {
                    if (str != string.Empty)
                    {
                        result.Add(str);
                        str = string.Empty;
                        onVar = false;
                        onNumeral = false;
                    }
                    result.Add(input[i].ToString());
                }
                else
                {
                    throw new ArgumentException("Wrong character");
                }
            }

            if (str != string.Empty)
                result.Add(str);

            return result.ToArray();
        }

        public Queue<string> GetRPN(string[] tokens)
        {
            Stack<string> operatorStack = new Stack<string>();
            Queue<string> outputQueue = new Queue<string>();

            for (int i = 0; i < tokens.Length; i++)
            {
                if (!operators.Contains(tokens[i]))
                {
                    outputQueue.Enqueue(tokens[i]);
                }
                else if (tokens[i].Equals("^"))
                {
                    operatorStack.Push(tokens[i]);
                }
                else if (OperatorPrecedences.TryGetValue(tokens[i], out int precedence) && precedence > 0)
                {
                    while(operatorStack.Count != 0 && OperatorPrecedences[operatorStack.Peek()] >= precedence)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Push(tokens[i]);
                }
                else if (tokens[i].Equals("("))
                {
                    operatorStack.Push(tokens[i]);
                }
                else if (tokens[i].Equals(")"))
                {
                    while (!operatorStack.Peek().Equals("("))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Pop();
                }
            }
            while(operatorStack.Count != 0)
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }

            return outputQueue;
        }

        public Operand[] GetExpression(Queue<string> rpnQueue)
        {
            var stack = new Stack<Operand>();

            while (rpnQueue.Count > 0)
            {
                var s = rpnQueue.Dequeue();

                if (decimal.TryParse(s, out var d))
                {
                    stack.Push(new Operand { Type = OperandType.Constant, Value = d });
                }
                else if(operators.Contains(s))
                {
                    var op1 = stack.Pop();
                    var op2 = stack.Pop();

                    var opType = OperationType.FromValue<OperationType>(s);
                    if (opType == OperationType.Substract)
                    {
                        op1.Value = -op1.Value;
                        opType = OperationType.Add;
                    }

                    if (op1.Operation?.OperationType == opType)
                    {
                        op1.Operation.Operands.Add(op2);
                        stack.Push(op1);
                    }
                    else if (op2.Operation?.OperationType == opType)
                    {
                        op2.Operation.Operands.Add(op1);
                        stack.Push(op2);
                    }
                    else
                    {
                        var op = new Operand
                        {
                            Type = OperandType.Operation,
                            Operation = new Operation
                            {
                                OperationType = opType,
                                Operands = new List<Operand> { op2, op1 }
                            },
                            Value = 1
                        };

                        stack.Push(op);
                    }
                }
                else
                {
                    stack.Push(new Operand { Type = OperandType.Variable, Name = s, Value = 1});
                }
            }

            return stack.ToArray();
        }

        public Operand Simplify(Operand input)
        {
            var operands = new List<Operand>();
            var multiplier = 1M;

            if (input.Type == OperandType.Operation)
            {
                foreach (var operand in input.Operation.Operands)
                {
                    var simpleOperand = Simplify(operand);
                    // SimplifyMultiplier(simpleOperand);
                    operands.Add(simpleOperand);
                }

                SimplifyConstants(operands, input.Operation.OperationType);
                SimplifyVariables(operands, input.Operation.OperationType);
                SimplifyCommonMultipliers(operands, input.Operation.OperationType);
            }
            else
            {
                return input;
            }

            foreach(var operand in operands)
            {
                SimplifyMultiplier(operand);
            }

            if (operands.Count == 1)
            {
                return operands[0];
            }

            if (input.Operation.OperationType == OperationType.Multiply)
            {
                var opConst = operands.SingleOrDefault(x => x.Type == OperandType.Constant);
                if (opConst != null)
                {
                    multiplier = opConst.Value.Value;
                    operands.Remove(opConst);
                }
            }

            Operand result;
            if (operands.Count == 1)
            {
                result = new Operand
                {
                    Name = operands[0].Name,
                    Operation = operands[0].Operation,
                    Type = operands[0].Type,
                    Value = input.Value * multiplier
                };
                // SimplifyMultiplier(result);
                // result.Value = input.Value * multiplier;
            }
            else
            {
                result = new Operand
                {
                    Type = OperandType.Operation,
                    Operation = new Operation 
                    {
                        Operands = operands,
                        OperationType = input.Operation.OperationType
                    },
                    Value = input.Value * multiplier
                };
            }

            // SimplifyMultiplier(result);
            return result;
        }

        private void SimplifyConstants(List<Operand> operands, OperationType operationType)
        {
            var constants = operands.Where(x => x.Type == OperandType.Constant).ToList();
            if (constants.Count >= 2)
            {
                var newOperand = new Operand
                {
                    Type = OperandType.Constant,
                    Value = constants[0].Value
                };

                for (var i = constants.Count - 1; i > 0; i--)
                {
                    newOperand.Value = operationType.Operation(
                        newOperand.Value.Value, constants[i].Value.Value);
                }
                operands.RemoveAll(x => x.Type == OperandType.Constant);
                operands.Add(newOperand);
            }
        }

        private void SimplifyVariables(List<Operand> operands, OperationType operationType)
        {
            if (operationType != OperationType.Add)
                return;

            var variableGroups = operands.Where(x => x.Type == OperandType.Variable).GroupBy(x => x.Name);

            foreach (var group in variableGroups)
            {
                var variables = group.ToList();
                if (variables.Count >= 2)
                {
                    var sum = variables.Sum(x => x.Value);
                    if (sum == 0)
                    {
                        operands.RemoveAll(x => variables.Contains(x));
                    }
                    else
                    {
                        var operand = new Operand
                        {
                            Type = OperandType.Operation,
                            Operation = new Operation
                            {
                                OperationType = OperationType.Multiply,
                                Operands = new List<Operand>()
                                {
                                    new Operand { Type = OperandType.Constant, Value = sum },
                                    new Operand { Type = OperandType.Variable, Name = group.Key }
                                }
                            }
                        };
                        operands.RemoveAll(x => variables.Contains(x));
                        operands.Add(operand);
                    }
                }
            }
        }

        private void SimplifyCommonMultipliers(List<Operand> operands, OperationType operationType)
        {
            if (operationType != OperationType.Add)
                return;

            for (var i = 0; i < operands.Count; i ++)
            {
                var operand = operands[i];

                if (operand.Type == OperandType.Constant)
                    continue;

                Operand[] varOp;
                if (operand.Type == OperandType.Variable)
                {
                    varOp = new Operand[] { operand };
                }
                else
                {
                    varOp = operand.Operation.Operands.Where(x => x.Type == OperandType.Variable).ToArray();
                }

                if (!varOp.Any())
                    continue;

                var varOps = operands.Where(
                    x => x.Type == OperandType.Operation
                    && x.Operation.Operands.Where(y => y.Type == OperandType.Variable).SequenceEqual(varOp)
                    && x.Operation.Operands.All(y => y.Type != OperandType.Operation)).ToList();

                if (varOps.Count > 1)
                {
                    // var consts = varOps.Select(x => x.Operation.Operands.SingleOrDefault(y => y.Type == OperandType.Constant)).ToList();
                    var newOperand = new Operand
                    {
                        Type = OperandType.Operation,
                        Operation = new Operation
                        {
                            OperationType = OperationType.Multiply,
                            Operands = new List<Operand>()
                            {
                                new Operand { Type = OperandType.Constant, Value = varOps.Sum(x => x?.Value ?? 1) }
                            }
                        }
                    };
                    // newOperand.Operation.Operands.AddRange(varOp);
                    // operands.Insert(operands.IndexOf(varOps.First()), newOperand);
                    varOps.First().Value = varOps.Sum(x => x?.Value ?? 1);
                    operands.RemoveAll(x => varOps.Skip(1).Contains(x));
                }
            }
        }

        private void SimplifyMultiplier(Operand operand)
        {
            if (operand.Type != OperandType.Operation ||
                operand.Operation.OperationType != OperationType.Multiply)
                return;

            var opConst = operand.Operation.Operands.Where(x => x.Type == OperandType.Constant).ToList();
            var op = operand.Operation.Operands.Where(x => x.Type != OperandType.Constant).ToList();
            if (opConst.Count == 1 && op.Count == 1)
            {
                operand.Type = op[0].Type;
                operand.Name = op[0].Name;
                operand.Operation = op[0].Operation;
                // operand = new Operand
                // {
                //     Name = op[0].Name,
                //     Operation = op[0].Operation,
                //     Type = op[0].Type,
                //     Value = opConst[0].Value
                // };
                // operand = op[0];
                operand.Value = opConst[0].Value;
            }
        }

    }
}