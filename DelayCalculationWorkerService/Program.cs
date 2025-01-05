using ConfigurationStuff.ServicesConfig;
using DelayCalculationWorkerService.Service;
using Grpc.Net.Client.Configuration;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;


namespace DelayCalculationWorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            ConfigurationStuff.ServicesConfig.ServiceConfig.AddSharedServices(builder.Services, builder.Configuration);

            //builder.Services.AddHostedService<DelayWorker>();
            
            //builder.Services.AddSingleton<RequestResponseTimeStorage>();

            var host = builder.Build();
            host.Run();
        }
    }
}