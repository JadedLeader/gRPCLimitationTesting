using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.ReturnModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayCalculationWorkerService.Service
{
    public class DelayCalculations
    {
     
        private readonly IRequestResponseTimeStorage _requestResponseTimeStorage;
        public DelayCalculations(IRequestResponseTimeStorage requestResponseTimeStorage)
        {
            _requestResponseTimeStorage = requestResponseTimeStorage;
        }

        public void CalculatingDelayFromTimeStorageDict()
        {
            var clientRequests =_requestResponseTimeStorage.ReturnClientRequests();

            var serverResponses =_requestResponseTimeStorage.ReturnServerResponse();

            if (clientRequests.Count == 0 || serverResponses.Count == 0)
            {
                Console.WriteLine($"Nothing to calculate - dict size of final calculations {_requestResponseTimeStorage.ReturnDelayCalculations().Count()}");
            }

            foreach (var timing in clientRequests)
            {
                if (serverResponses.TryGetValue(timing.Key, out var timingValue))
                {

                    var calculation = Convert.ToDateTime(timing.Value.TimeOfRequest.Value) - Convert.ToDateTime(timingValue.TimeOfRequest.Value);

                    Console.WriteLine($"ID {timing.Key} has delay {calculation}");

                    var unaryRequest = new UnaryInfo()
                    {
                        Delay = calculation.Duration(),
                        LengthOfData = 0,
                        TypeOfData = ""

                    };

                    _requestResponseTimeStorage.AddFinalTimeToDict(timing.Key, unaryRequest);

                    clientRequests.Remove(timing.Key);
                    serverResponses.Remove(timing.Key);
                }
            }

            var delayCalculations = _requestResponseTimeStorage.ReturnDelayCalculations();
        }

        private int LengthOfRequest(UnaryInfo info)
        {

            var temp = info.LengthOfData.Value.

            if (info.LengthOfData == null)
            {
                Console.WriteLine($"There was no string data passed along with this request");
            }

            return info.LengthOfDa
        }


    }
}
