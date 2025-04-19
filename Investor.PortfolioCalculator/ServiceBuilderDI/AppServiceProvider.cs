using Investor.PortfolioCalculator.Business.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Investor.PortfolioCalculator.ServiceBuilderDI
{
    public class AppServiceProvider
    {
        private ServiceProvider serviceProvider { get; set; } // DI Container
        public AppServiceProvider()
        {
            serviceProvider = new ServiceCollection()
                .AddSingleton<IPortfolioCalculatorLogic, PortfolioCalculatorLogic>() // Business Layer Dependency
                .AddSingleton<IDataRepository, DataRepository>() // Data Layer Dependency
                .BuildServiceProvider();
        }

        public T GetService<T>() where T : class
        {
            return serviceProvider.GetService<T>() ?? throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        }
    }
}
