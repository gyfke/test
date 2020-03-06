using Algebra.Computations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algebra.Tests
{
    [TestClass]
    public class GetTokensTests
    {
        private IParser parser;

        [TestInitialize]
        public void Initialize()
        {
            parser = new Parser();
        }

        [DataRow("123")]
        [DataRow("123.456")]
        [DataRow("0.123")]
        [DataRow("x")]
        [TestMethod]
        public void GetTokens_WhenExpressionIsSingleConstant_ReturnSingleToken(string input)
        {
            // Assert

            // Act
            var result = parser.GetTokens(input);
            
            // Arrange
            var token = result.Should().ContainSingle().Subject;
            token.Should().Be(input);
        }
        
        [DataRow("1a", "1", "a")]
        [DataRow("1.5x", "1.5", "x")]
        [DataRow("0.5y", "0.5", "y")]
        [DataRow("ax", "a", "x")]
        [TestMethod]
        public void GetTokens_WhenExpressionIsImplicitMultiplication_ReturnThreeTokens(string input, string op1, string op2)
        {
            // Assert

            // Act
            var result = parser.GetTokens(input);
            
            // Arrange
            result.Length.Should().Be(3);
            result[0].Should().Be(op1);
            result[1].Should().Be("*");
            result[2].Should().Be(op2);
        }

        [DataRow("1 + a", "1", "+", "a")]
        [DataRow("1.5 - x", "1.5", "-", "x")]
        [DataRow("2 * b", "2", "*", "b")]
        [DataRow("2.5 / y", "2.5", "/", "y")]
        [TestMethod]
        public void GetTokens_WhenExpressionContainsSingleOperation_ReturnThreeTokens(string input, string op1, string op, string op2)
        {
            // Assert

            // Act
            var result = parser.GetTokens(input);
            
            // Arrange
            result.Length.Should().Be(3);
            result[0].Should().Be(op1);
            result[1].Should().Be(op);
            result[2].Should().Be(op2);
        }

        [DataRow("a+b", "a", "+", "b")]
        [DataRow("x-z", "x", "-", "z")]
        [DataRow("a*b", "a", "*", "b")]
        [DataRow("a/b", "a", "/", "b")]
        [TestMethod]
        public void GetTokens_WhenExpressionContainsTwoVariables_ReturnThreeTokens(string input, string op1, string op, string op2)
        {
            // Assert

            // Act
            var result = parser.GetTokens(input);
            
            // Arrange
            result.Length.Should().Be(3);
            result[0].Should().Be(op1);
            result[1].Should().Be(op);
            result[2].Should().Be(op2);
        }

        [DataRow("1+a", "1", "a")]
        [DataRow("1.5+x", "1.5", "x")]
        [DataRow("2 + b", "2", "b")]
        [DataRow("2.5   +   y", "2.5", "y")]
        [TestMethod]
        public void GetTokens_WhenExpressionContainsSpaces_IgnoreSpaces(string input, string op1, string op2)
        {
            // Assert

            // Act
            var result = parser.GetTokens(input);
            
            // Arrange
            result.Length.Should().Be(3);
            result[0].Should().Be(op1);
            result[1].Should().Be("+");
            result[2].Should().Be(op2);
        }

        [TestMethod]
        public void GetTokens_ComplexExpression()
        {
            // Assert
            var input = "a^2+2ab-b^2";

            // Act
            var result = parser.GetTokens(input);
            
            // Arrange
            result.Length.Should().Be(13);
            result.Should().BeEquivalentTo(new string[] {
                "a", "^", "2", "+", "2", "*", "a", "*", "b", "-", "b", "^", "2"
            });
        }

    }
}
