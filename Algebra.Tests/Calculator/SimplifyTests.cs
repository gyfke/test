using System;
using Algebra.Computations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algebra.Tests.Calculator
{
    [TestClass]
    public class SimplifyTests
    {
        private ICalculator calculator;

        [TestInitialize]
        public void Initialize()
        {
            calculator = new SimpleCalculator();
        }

        [DataRow("a+a+a=1+2+3", "3a=6")]
        [DataRow("3a+2a-a=5-4-3-2-1", "4a=-5")]
        [DataRow("x^2+3.5xy+y=y^2-xy+y", "x^2+4.5xy-y^2=0")]
        [TestMethod]
        public void Simplify(string input, string expected)
        {
            // Arrange

            // Act
            var result = calculator.Simplify(input);

            // Assert
            result.Should().Be(expected);
        }
    }
}
