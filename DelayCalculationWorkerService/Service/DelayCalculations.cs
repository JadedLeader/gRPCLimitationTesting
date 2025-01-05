using Serilog;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.ReturnModels.ReturnTypes;
using SharedCommonalities.TimeStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DelayCalculationWorkerService.Service
{
    public class DelayCalculations
    {

        private readonly RequestResponseTimeStorage _requestResponseTimeStorage;
        private readonly object _dictLock = new object(); // Shared lock object for consistency

        public DelayCalculations(RequestResponseTimeStorage requestResponseTimeStorage)
        {
            _requestResponseTimeStorage = requestResponseTimeStorage;
        }

        public void CalculatingDelayFromTimeStorageDict()
        {
            // Lock to ensure consistency when accessing both dictionaries
            lock (_dictLock)
            {
                var clientRequests = _requestResponseTimeStorage.ReturnConcurrentDictLock(_requestResponseTimeStorage._ClientRequestTiming);
                var serverResponses = _requestResponseTimeStorage.ReturnConcurrentDictLock(_requestResponseTimeStorage._ServerResponseTiming);

                Console.WriteLine($"Client Requests Count -> {clientRequests.Count}");
                Console.WriteLine($"Server Responses Count -> {serverResponses.Count}");

                if (clientRequests.Count == 0 || serverResponses.Count == 0)
                {
                    Console.WriteLine($"No data to calculate. Skipping delay calculations.");
                    return;
                }

                var keysToRemove = new List<ClientMessageId>();

                // Iterate through client requests and calculate delay safely
                foreach (var timing in clientRequests)
                {
                    var verifyingServerResponseId = serverResponses.Keys
                        .FirstOrDefault(entry =>
                            entry.ClientId == timing.Key.ClientId && entry.MessageId == timing.Key.MessageId);

                    if (verifyingServerResponseId != null && serverResponses.TryGetValue(verifyingServerResponseId, out var timingValue))
                    {
                        var calc = Convert.ToDateTime(timingValue.TimeOfRequest.Value) - Convert.ToDateTime(timing.Value.TimeOfRequest.Value);
                        Log.Information($"Client ID : {timing.Key.ClientId} with message ID {timing.Key.MessageId} had delay {calc}");

                        // Ensure thread safety when updating the shared dictionary
                        if (!_requestResponseTimeStorage._ActualDelayCalculations.ContainsKey(timing.Key))
                        {
                            var unaryRequest = new UnaryInfo
                            {
                                Delay = calc.Duration(),
                                LengthOfData = timingValue.LengthOfData,
                                TypeOfData = timing.Value.TypeOfData,
                                DataContents = timing.Value.DataContents
                            };

                            _requestResponseTimeStorage.AddToConcurrentDictLock(_requestResponseTimeStorage._ActualDelayCalculations, timing.Key, unaryRequest);
                        }

                        keysToRemove.Add(timing.Key);
                    }
                }

                // Remove keys after calculation inside the same lock
                foreach (var key in keysToRemove)
                {
                    _requestResponseTimeStorage.RemoveFromConcurrentDictLock(_requestResponseTimeStorage._ClientRequestTiming, key);
                    _requestResponseTimeStorage.RemoveFromConcurrentDictLock(_requestResponseTimeStorage._ServerResponseTiming, key);
                }

                Console.WriteLine($"Final Client Requests Count -> {_requestResponseTimeStorage._ClientRequestTiming.Count}");
                Console.WriteLine($"Final Server Responses Count -> {_requestResponseTimeStorage._ServerResponseTiming.Count}");
            }
        }
    }
}
