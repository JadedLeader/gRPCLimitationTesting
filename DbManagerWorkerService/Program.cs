global using SharedCommonalities;
using DbManagerWorkerService.Repos;
using Microsoft.EntityFrameworkCore;
using DbManagerWorkerService.Interfaces;
using DbManagerWorkerService.Services;
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Repositories;


namespace DbManagerWorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            ConfigurationStuff.ServicesConfig.ServiceConfig.AddSharedServices(builder.Services, builder.Configuration);

            //builder.Services.AddHostedService<DbManagerWorker>();

            builder.Services.AddScoped<ICommunicationDelayService, CommunicationDelayService>();

            var host = builder.Build();

            Console.WriteLine($"host is running");

            host.Run();
        }
    }
}