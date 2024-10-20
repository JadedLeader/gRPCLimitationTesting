using ClientManagementWorkerService.Interfaces;
using SharedCommonalities.UsefulFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientManagementWorkerService.Services;
using SharedCommonalities.Storage;

namespace ClientManagementWorker
{
    public class ClientManagerWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public ClientManagerWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using(var scope = _serviceProvider.CreateScope())
                    {
                        var clientManagementScoped = scope.ServiceProvider.GetRequiredService<IClientManagementService>();

                        if(Settings.GetRemoveClientState())
                        {
                            //await clientManagementScoped.RemovingSentRequestsFromClientPool();

                            await clientManagementScoped.thing();
                        }

                        Console.WriteLine(ClientStorage.ReturnDictionary().Count);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
                catch(Exception ex) 
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }
    }
}
