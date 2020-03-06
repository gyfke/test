using System;
using System.IO;
using System.Threading.Tasks;
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
        public ActionResult Simplify([FromForm]string expression)
        {
            return new OkObjectResult(calculator.Simplify(expression));
        }

        [HttpPost("simplifyFile")]
        public async Task<ActionResult> SimplifyFileAsync()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var expressions = body.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var result = await calculator.SimplifyAsync(expressions);
            
                using (var sw = new StreamWriter(Response.Body))
                {
                    var output = new FileStreamResult(sw.BaseStream, "text/plain");
                    foreach (var expression in result)
                    {
                        await sw.WriteLineAsync(expression);
                    }

                    return output;
                }
            }
        }
    }
}
