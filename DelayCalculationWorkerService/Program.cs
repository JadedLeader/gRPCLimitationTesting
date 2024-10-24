using DelayCalculationWorkerService.Service;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.ServicesConfig;

namespace DelayCalculationWorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            
            ServiceConfig.AddSharedServices(builder.Services);

            builder.Services.AddHostedService<DelayWorker>();
            
            builder.Services.AddSingleton<RequestResponseTimeStorage>();

            var host = builder.Build();
            host.Run();
        }
    }
}