using DelayCalculationWorkerService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayCalculationWorkerService
{
    public class DelayWorker : BackgroundService
    {
        private readonly DelayCalculations _delayCalculations;
        public DelayWorker(DelayCalculations delayCalculations)
        {
            _delayCalculations = delayCalculations;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    _delayCalculations.CalculatingDelayFromTimeStorageDict();

                    await Task.Delay(TimeSpan.FromMilliseconds(1), stoppingToken);
                } 
                catch (Exception ex)
                {
                    Console.WriteLine($"Something happened in the worker -> {ex.Message}");
                }
            }
        }
    }
}
