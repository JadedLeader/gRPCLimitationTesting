using DelayCalculationWorkerService.Service;
using SharedCommonalities.UsefulFeatures;
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
        private bool _isCalculating = false;
        private readonly object _workerLock = new object();

        public DelayWorker(DelayCalculations delayCalculations)
        {
            _delayCalculations = delayCalculations;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                lock (_workerLock)
                {
                    if (_isCalculating) continue;
                    _isCalculating = true;
                }

                try
                {
                    _delayCalculations.CalculatingDelayFromTimeStorageDict();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in DelayWorker: {ex.Message}");
                }
                finally
                {
                    _isCalculating = false;
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
