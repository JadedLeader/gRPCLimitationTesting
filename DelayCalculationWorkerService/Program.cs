using DelayCalculationWorkerService.Service;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;

namespace DelayCalculationWorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            
            builder.Services.AddHostedService<DelayWorker>();
            builder.Services.AddSingleton<DelayCalculations>();
            builder.Services.AddSingleton<RequestResponseTimeStorage>();

            var host = builder.Build();
            host.Run();
        }
    }
}