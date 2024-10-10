using DelayCalculationWorker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayCalculationWorker
{
    public class WorkerService : BackgroundService
    {

        private readonly IDelayCalculations _DelayCalculations;
        public WorkerService(IDelayCalculations delayCalculations)
        {
            _DelayCalculations = delayCalculations;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    //this literally just needs to run a method that takes no params so that it can work out the times 
                    _DelayCalculations.DoingTimeCalculations();
                }
                catch
                {

                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

    }
}
