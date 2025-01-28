using ConfigurationStuff.DbContexts;
using ConfigurationStuff.Interfaces.Queries;
using ConfigurationStuff.Interfaces.Repos;
using ConfigurationStuff.Queries;
using ConfigurationStuff.Repositories;
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SharedCommonalities.Storage;
using SharedCommonalities.TimeStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConfigurationStuff.ServicesConfig
{
    public static class ServiceConfig
    {


        /// <summary>
        /// The purpose of this class is to define the scopes of the services that are shared between many project files 
        /// This has been done because, since there are many project files, we want to use the same instances between the different projects 
        /// Therefore we are instantiating (registering) them in one place
        /// </summary>
        /// <param name="services"></param>  
        public static void AddSharedServices(IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddSingleton<ClientStorage>();
            services.AddSingleton<RequestResponseTimeStorage>();
            services.AddSingleton<ThroughputStorage>();
            


             var connectionString = configuration.GetConnectionString("DbConnection");
              services.AddDbContext<DataContexts>(options =>
              {
                  options.UseSqlServer(connectionString);
                  options.UseSqlServer(connectionString);
                 // options.EnableSensitiveDataLogging();
                 // options.EnableDetailedErrors();
                  /*options.LogTo(
                      Console.WriteLine,
                      new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted },
                      LogLevel.Information
                      ); */
              }); 

            services.AddScoped<IDataContexts>(sp => sp.GetRequiredService<DataContexts>());

            // Add repository lifetimes
            services.AddScoped<IAccountRepo, AccountRepo>();
            services.AddScoped<IAuthTokenRepo, AuthTokenRepo>();
            services.AddScoped<IClientInstanceRepo, ClientInstanceRepo>();
            services.AddScoped<ISessionRepo, SessionRepo>();
            services.AddScoped<IDelayCalcRepo, delayCalcRepo>();
            services.AddScoped<IDataQueries, DataQueries>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("C:\\Users\\joshy\\source\\repos\\gRPCLimitationTesting\\SharedCommonalities\\Logs\\logger.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();


        }

    }
}
