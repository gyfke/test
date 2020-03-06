using System.Threading.Tasks;

namespace Algebra.Computations
{
    public interface ICalculator
    {
        string Simplify(string input);
        Task<string[]> SimplifyAsync(string[] input);
    }
}