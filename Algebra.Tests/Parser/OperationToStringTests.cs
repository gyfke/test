using Algebra.Computations;
using Algebra.Computations.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algebra.Tests
{
    [TestClass]
    public class OperationToStringTests
    {
        private IParser parser;

        [TestInitialize]
        public void Initialize()
        {
            parser = new Parser();
        }

        [TestMethod]
        public void OperationToString_ConstantSum()
        {
            // Assert
            var tokens = parser.GetTokens("1+2+3+4+5");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);
            var simple = parser.Simplify(operands[0]);

            // Act
            var result = simple.ToString();
            
            // Arrange
            result.Should().Be("15");
        }

        [TestMethod]
        public void OperationToString_ConstantMultiplication()
        {
            // Assert
            var tokens = parser.GetTokens("1*2*3*4*5");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);
            var simple = parser.Simplify(operands[0]);

            // Act
            var result = simple.ToString();
            
            // Arrange
            result.Should().Be("120");
        }

        [TestMethod]
        public void OperationToString_VariablesSum()
        {
            // Assert
            var tokens = parser.GetTokens("a+a+a+a+a");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);
            var simple = parser.Simplify(operands[0]);

            // Act
            var result = simple.ToString();
            
            // Arrange
            result.Should().Be("5a");
        }

        [TestMethod]
        public void OperationToString_VariablesCommonMultiplier()
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

        [TestMethod]
        public void OperationToString_1()
        {
            // Assert
            var tokens = parser.GetTokens("x^2 + 3.5xy + y - y^2 + xy - y");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);
            var simple = parser.Simplify(operands[0]);

            // Act
            var result = simple.ToString();

            // Arrange
            result.Should().Be("x^2+4.5xy-y^2");
        }

        [TestMethod]
        public void OperationToString_2()
        {
            // Assert
            var tokens = parser.GetTokens("4x-2-4x-2x+2-8x+24");
            var rpn = parser.GetRPN(tokens);
            var operands = parser.GetExpression(rpn);
            var simple = parser.Simplify(operands[0]);

            // Act
            var result = simple.ToString();

            // Arrange
            result.Should().Be("24-10x");
        }
    }
}
