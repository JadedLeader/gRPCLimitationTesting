global using DbManagerWorkerService.DbModels;
using DbManagerWorkerService.Repos;
using DbManagerWorkerService.DbContexts;
using Microsoft.EntityFrameworkCore;
using DbManagerWorkerService.Interfaces;
using DbManagerWorkerService.Services;
using DbManagerWorkerService.Interfaces.DataContext;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.Storage;
using DbManagerWorkerService.Interfaces.Repos;
using SharedCommonalities.ServicesConfig;
using DbManagerWorkerService.Repositories;

namespace DbManagerWorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddHostedService<DbManagerWorker>();

            ServiceConfig.AddSharedServices(builder.Services);

            builder.Services.AddTransient<IDataContexts, DataContexts>();

            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<IAuthTokenRepo, AuthTokenRepo>();
            builder.Services.AddScoped<IClientInstanceRepo, ClientInstanceRepo>();
            builder.Services.AddScoped<IDelayCalcRepo, delayCalcRepo>();
            builder.Services.AddScoped<ISessionRepo, SessionRepo>();

            builder.Services.AddSingleton<ICommunicationDelayRepo, CommunicationDelayRepo>();
            builder.Services.AddSingleton<ICommunicationDelayService, CommunicationDelayService>();

            var connectionString = builder.Configuration.GetConnectionString("DbConnection");
            builder.Services.AddDbContext<DataContexts>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            var host = builder.Build();



            Console.WriteLine($"host is running");

            host.Run();
        }
    }
}