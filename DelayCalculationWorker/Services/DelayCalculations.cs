using SharedCommonalities.Interfaces.TimeStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCommonalities.ReturnModels;
using DelayCalculationWorker.Interfaces;

namespace DelayCalculationWorker.Services
{
    public class DelayCalculations : IDelayCalculations
    {

        


        private readonly IRequestResponseTimeStorage _RequestResponseTimeStorage;
        public DelayCalculations(IRequestResponseTimeStorage requestResponseTimeStorage)
        {
            _RequestResponseTimeStorage = requestResponseTimeStorage;
        }

        public void DoingTimeCalculations()
        {

            var clientRequests = _RequestResponseTimeStorage.ReturnClientRequests();

            var serverResponses = _RequestResponseTimeStorage.ReturnServerResponse();

            if(clientRequests.Count == 0 || serverResponses.Count == 0)
            {
                Console.WriteLine($"Nothing in the dictionaries");
            }

            foreach (var timing in clientRequests)
            {
                if (serverResponses.TryGetValue(timing.Key, out var timingValue))
                {
                    var calculation = Convert.ToDouble(timing.Value) - Convert.ToDouble(timingValue);

                    _RequestResponseTimeStorage.AddFinalTimeTODict(timing.Key, calculation);
                }
            }

            var delayCalculations = _RequestResponseTimeStorage.ReturnDelayCalculations();

            foreach (var timing in delayCalculations)
            {
                Console.WriteLine($"Guid:  {timing.Key} delay: {timing.Value}");
            }

        }

        public async Task PollingCLientRequestDict(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {

            }
        }


    }
}
