using DbManagerWorkerService.Interfaces;

namespace DbManagerWorkerService
{
    public class DbManagerWorker : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;

     
        public DbManagerWorker(IServiceProvider scope)
        {
            
            
            _serviceProvider = scope;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using(var scope = _serviceProvider.CreateScope())
                    {
                          var communicationDelayService = scope.ServiceProvider.GetRequiredService<ICommunicationDelayService>();

                          //await communicationDelayService.AddingDelayCalculationsToDb();

                          //await communicationDelayService.EmptyTable();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error in db manager worker -> {ex.Message}");
                }
            }

            
        }
    }
}
