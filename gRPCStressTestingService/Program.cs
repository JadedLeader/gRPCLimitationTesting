using gRPCStressTestingService.Implementations;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.Services;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;
using DelayCalculationWorkerService;
using DelayCalculationWorkerService.Service;
using Microsoft.EntityFrameworkCore;
using DbManagerWorkerService;
using DbManagerWorkerService.Services;
using DbManagerWorkerService.Interfaces;
using DbManagerWorkerService.DbContexts;
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Repos;

namespace gRPCStressTestingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Listen(System.Net.IPAddress.Loopback, 5000);
                serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024;

            });

            // Add services to the container.
            builder.Services.AddGrpc(options =>
            {
                options.MaxReceiveMessageSize = 100 * 1024 * 1024;
            });

            builder.Services.AddScoped<IUnaryService, UnaryService>();
            builder.Services.AddScoped<UnaryImplementation>();
            builder.Services.AddSingleton<DelayCalculations>(); 
            builder.Services.AddHostedService<DbManagerWorker>();
            builder.Services.AddTransient<IDataContexts, DataContexts>();
            builder.Services.AddSingleton<ICommunicationDelayRepo, CommunicationDelayRepo>();
            builder.Services.AddSingleton<ICommunicationDelayService, CommunicationDelayService>();
            builder.Services.AddSingleton<RequestResponseTimeStorage>();

            builder.Services.AddHostedService<DelayWorker>();

            var app = builder.Build();
            
            app.UseRouting();

            app.MapGrpcService<UnaryImplementation>();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}