using Investor.PortfolioCalculator.Business.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Investor.PortfolioCalculator.ServiceBuilderDI
{
    public class AppServiceProvider
    {
        private ServiceProvider serviceProvider { get; set; }//DI Container
        public AppServiceProvider()
        {
            serviceProvider = new ServiceCollection()
            .AddSingleton<IPortfolioCalculatorLogic, PortfolioCalculatorLogic>()// Business Layer Dependency
            .AddSingleton<IDataRepository, DataRepository>()// Data Layer Dependency
            .BuildServiceProvider();
        }
        public T GetService<T>()
        {
            return serviceProvider.GetService<T>();
        }
    }
}
