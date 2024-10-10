using DelayCalculationWorkerService.Service;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;

namespace DelayCalculationWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            

            builder.Services.AddHostedService<DelayWorker>();
            builder.Services.AddSingleton<DelayCalculations>();
            builder.Services.AddSingleton<IRequestResponseTimeStorage, RequestResponseTimeStorage>();

            var host = builder.Build();
            host.Run();
        }
    }
}