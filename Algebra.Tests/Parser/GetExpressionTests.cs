using Algebra.Computations;
using Algebra.Computations.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algebra.Tests
{
    [TestClass]
    public class GetExpressionTests
    {
        private IParser parser;

        [TestInitialize]
        public void Initialize()
        {
            parser = new Parser();
        }

        [TestMethod]
        public void GetExpression_1()
        {
            // Assert
            var tokens = parser.GetTokens("a+2b");
            var rpn = parser.GetRPN(tokens);

            // Act
            var result = parser.GetExpression(rpn);
            
            // Arrange
            var operation = result.Should().ContainSingle().Subject;
            operation.Operation.OperationType.Should().Be(OperationType.Add);
        }
    }
}
