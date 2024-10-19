using ClientManagementWorkerService.Services;
using ClientManagementWorkerService.Interfaces;
using SharedCommonalities.Storage;
using SharedCommonalities.UsefulFeatures;

namespace ClientManagementWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddHostedService<ClientManagerWorker>();
            builder.Services.AddSingleton<IClientManagementService, ClientManagementService>();
           // builder.Services.AddSingleton<ClientStorage>();
         

            var host = builder.Build();
            host.Run();
        }
    }
}