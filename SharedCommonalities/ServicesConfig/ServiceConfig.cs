using Microsoft.Extensions.DependencyInjection;
using SharedCommonalities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCommonalities.TimeStorage;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SharedCommonalities.ServicesConfig
{
    public static class ServiceConfig
    {


        /// <summary>
        /// The purpose of this class is to define the scopes of the services that are shared between many project files 
        /// This has been done because, since there are many project files, we want to use the same instances between the different projects 
        /// Therefore we are instantiating (registering) them in one place
        /// </summary>
        /// <param name="services"></param>  
        public static void AddSharedServices(IServiceCollection services)
        {
            
            services.AddSingleton<ClientStorage>();
            services.AddSingleton<RequestResponseTimeStorage>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("C:\\Users\\joshy\\source\\repos\\gRPCLimitationTesting\\SharedCommonalities\\Logs\\logger.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();


        }

    }
}
