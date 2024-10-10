using DelayCalculationWorker.Interfaces;
using DelayCalculationWorker.Services;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;

namespace DelayCalculationWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddSingleton<IRequestResponseTimeStorage, RequestResponseTimeStorage>();

            builder.Services.AddSingleton<IDelayCalculations, DelayCalculations>();
         
            builder.Services.AddHostedService<WorkerService>();

            var host = builder.Build();
            host.Run();
        }
    }
}