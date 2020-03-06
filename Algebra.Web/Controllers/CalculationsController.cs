using System;
using Algebra.Computations;
using Microsoft.AspNetCore.Mvc;

namespace Algebra.Web.Controllers
{
    [Controller]
    [Route("calculation")]
    public class CalculationsController : Controller
    {
        private readonly ICalculator calculator;

        public CalculationsController(ICalculator calculator)
        {
            this.calculator = calculator;
        }

        [HttpPost("simplify")]
        public string Simplify([FromForm]string expression)
        {
            return calculator.Simplify(expression);
        }
    }
}
