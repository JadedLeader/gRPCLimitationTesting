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

                    Console.WriteLine("-----------------------------------------------------");
                    Console.WriteLine($"CURRENT AMOUNT OF ACTIVE CLIENTS THIS IS SERVER SIDE -> {Settings.GetNumberOfActiveClients()}");
                    Console.WriteLine("-----------------------------------------------------");

                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                } 
                catch (Exception ex)
                {
                    Console.WriteLine($"Something happened in the worker -> {ex.Message}");
                }
            }
        }
    }
}
