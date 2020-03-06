using Algebra.Computations;
using Algebra.Computations.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algebra.Tests
{
    [TestClass]
    public class SimplifyTests
    {
        private IParser parser;

        [TestInitialize]
        public void Initialize()
        {
            parser = new Parser();
        }

        [TestMethod]
        public void Simplify_ConstantSum()
        {
            // Assert
            var tokens = parser.GetTokens("1+2+3+4+5");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);

            // Act
            var result = parser.Simplify(operands[0]);
            
            // Arrange
            result.Type.Should().Be(OperandType.Constant);
            result.Value.Should().Be(15);
        }

        [TestMethod]
        public void Simplify_ConstantSubstract()
        {
            // Assert
            var tokens = parser.GetTokens("20-10-5-3");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);

            // Act
            var result = parser.Simplify(operands[0]);
            
            // Arrange
            result.Type.Should().Be(OperandType.Constant);
            result.Value.Should().Be(2);
        }

        [TestMethod]
        public void Simplify_ConstantSumSubstract()
        {
            // Assert
            var tokens = parser.GetTokens("10-9+8-7+6-5+4-3+2-1");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);

            // Act
            var result = parser.Simplify(operands[0]);
            
            // Arrange
            result.Type.Should().Be(OperandType.Constant);
            result.Value.Should().Be(5);
        }

        [TestMethod]
        public void Simplify_ConstantMultiplication()
        {
            // Assert
            var tokens = parser.GetTokens("1*2*3*4*5");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);

            // Act
            var result = parser.Simplify(operands[0]);
            
            // Arrange
            result.Type.Should().Be(OperandType.Constant);
            result.Value.Should().Be(120);
        }

        [TestMethod]
        public void Simplify_VariablesSum()
        {
            // Assert
            var tokens = parser.GetTokens("a+a+a+a+a");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);

            // Act
            var result = parser.Simplify(operands[0]);
            
            // Arrange
            result.Type.Should().Be(OperandType.Variable);
            result.Name.Should().Be("a");
            result.Value.Should().Be(5M);
        }

        [TestMethod]
        public void Simplify_VariablesCommonMultiplier()
        {
            // Assert
            var tokens = parser.GetTokens("2ab+3ab");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);

            // Act
            var result = parser.Simplify(operands[0]);
            
            // Arrange
            result.Type.Should().Be(OperandType.Operation);
            result.Operation.OperationType.Should().Be(OperationType.Multiply);
            result.Operation.Operands.Should().Contain(x => x.Type == OperandType.Variable && x.Name == "a");
            result.Operation.Operands.Should().Contain(x => x.Type == OperandType.Variable && x.Name == "b");
            result.Value.Should().Be(5M);
        }

        [TestMethod]
        public void Simplify_VariablesCommonMultiplier_2()
        {
            // Assert
            var tokens = parser.GetTokens("5ab-3ab");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);

            // Act
            var result = parser.Simplify(operands[0]);
            
            // Arrange
            result.Type.Should().Be(OperandType.Operation);
            result.Operation.OperationType.Should().Be(OperationType.Multiply);
            result.Operation.Operands.Should().Contain(x => x.Type == OperandType.Variable && x.Name == "a");
            result.Operation.Operands.Should().Contain(x => x.Type == OperandType.Variable && x.Name == "b");
            result.Value.Should().Be(2M);
        }


        [TestMethod]
        public void SimplifyToString_VariablesCommonMultiplier()
        {
            // Assert
            var tokens = parser.GetTokens("2ab+3ab");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);
            var simple = parser.Simplify(operands[0]);

            // Act
            var result = simple.ToString();

            // Arrange
            result.Should().Be("5ab");
        }
    }
}
