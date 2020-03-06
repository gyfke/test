using Algebra.Computations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algebra.Tests
{
    [TestClass]
    public class GetRPNTests
    {
        private IParser parser;

        [TestInitialize]
        public void Initialize()
        {
            parser = new Parser();
        }

        [TestMethod]
        public void GetRPN_1()
        {
            // Assert
            var tokens = parser.GetTokens("a+2b");

            // Act
            var result = parser.GetRPN(tokens);
            
            // Arrange
            result.Should().BeEquivalentTo(new string[] { "a", "2", "b", "*", "+" });
        }

        [TestMethod]
        public void GetRPN_2()
        {
            // Assert
            var tokens = parser.GetTokens("(a+2)*b");

            // Act
            var result = parser.GetRPN(tokens);
            
            // Arrange
            result.Should().BeEquivalentTo(new string[] { "a", "2", "+", "b", "*" });
        }

        [TestMethod]
        public void GetRPN_3()
        {
            // Assert
            var tokens = parser.GetTokens("a^2 + 2ab + b^2");

            // Act
            var result = parser.GetRPN(tokens);
            
            // Arrange
            result.Should().BeEquivalentTo(new string[] {"a", "2", "^", "2", "a", "*", "b", "*", "+", "b", "2", "^", "+"});
        }

    }
}
