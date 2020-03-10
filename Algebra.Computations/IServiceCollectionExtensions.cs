using System;
using Microsoft.Extensions.DependencyInjection;

namespace Algebra.Computations
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddComputationServices(this IServiceCollection services)
        {
            services.AddTransient<IParser, Parser>();
            return services;
        }
    }
}
