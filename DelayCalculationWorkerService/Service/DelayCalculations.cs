using Serilog;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.ReturnModels.ReturnTypes;
using SharedCommonalities.TimeStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayCalculationWorkerService.Service
{
    public class DelayCalculations
    {

        private readonly RequestResponseTimeStorage _requestResponseTimeStorage;

        public DelayCalculations(RequestResponseTimeStorage requestResponseTimeStorage)
        {
            _requestResponseTimeStorage = requestResponseTimeStorage;
        }

        public void CalculatingDelayFromTimeStorageDict()
        {
            var clientRequests = _requestResponseTimeStorage.ReturnDictionary(_requestResponseTimeStorage._ClientRequestTiming);

            var serverResponses = _requestResponseTimeStorage.ReturnDictionary(_requestResponseTimeStorage._ServerResponseTiming);
               
            if (clientRequests.Count == 0 || serverResponses.Count == 0)
            {
                Console.WriteLine($"Nothing to calculate - dict size of final calculations {_requestResponseTimeStorage.ReturnDictionary(_requestResponseTimeStorage._ServerResponseTiming).Count()}");
            }

            foreach (var timing in clientRequests)
            {
                if (serverResponses.TryGetValue(timing.Key, out var timingValue))
                {
    
                    var calc = Convert.ToDateTime(timingValue.TimeOfRequest.Value) - Convert.ToDateTime(timing.Value.TimeOfRequest.Value);

                    Log.Information($"ID {timing.Key} has delay {calc}");

                    var unaryRequest = new UnaryInfo()
                    {
                        Delay = calc.Duration(),
                        LengthOfData = 0,
                        TypeOfData = timing.Value.TypeOfData

                    };

                    _requestResponseTimeStorage.AddToDictionary(_requestResponseTimeStorage._ActualDelayCalculations, timing.Key, unaryRequest);

                    clientRequests.Remove(timing.Key);
                    serverResponses.Remove(timing.Key);
                }
            }
        }

        private int LengthOfRequest(UnaryInfo info)
        {

            /*var temp = info.LengthOfData.Value.

            if (info.LengthOfData == null)
            {
                Console.WriteLine($"There was no string data passed along with this request");
            }

            return info.LengthOfDa */

            throw new NotImplementedException();
        }


    }
}
