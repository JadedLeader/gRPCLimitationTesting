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

        public DelayCalculations(RequestResponseTimeStorage requestResponseTimeStorage)
        {
            _requestResponseTimeStorage = requestResponseTimeStorage;
        }

        public void CalculatingDelayFromTimeStorageDict()
        {
            var clientRequests = _requestResponseTimeStorage.ReturnDictionary(_requestResponseTimeStorage._ClientRequestTiming);

            Console.WriteLine($"amount of things in clients request -> {clientRequests.Count}");

            var serverResponses = _requestResponseTimeStorage.ReturnDictionary(_requestResponseTimeStorage._ServerResponseTiming);

            Console.WriteLine($"amunt of things in server responses -> {clientRequests.Count}");

            if (clientRequests.Count == 0 || serverResponses.Count == 0)
            {
                Console.WriteLine($"Nothing to calculate - dict size of final calculations {_requestResponseTimeStorage.ReturnDictionary(_requestResponseTimeStorage._ServerResponseTiming).Count()}");
            }

            foreach (var timing in clientRequests)
            {

                var verifyingServerResponseId = serverResponses.Keys.FirstOrDefault(entry =>
                entry.ClientId == timing.Key.ClientId && entry.MessageId == timing.Key.MessageId);

                if (verifyingServerResponseId != null && serverResponses.TryGetValue(verifyingServerResponseId, out var timingValue))
                {
    
                    var calc = Convert.ToDateTime(timingValue.TimeOfRequest.Value) - Convert.ToDateTime(timing.Value.TimeOfRequest.Value);

                    Log.Information($"Client ID : {timing.Key.ClientId} with message ID { timing.Key.MessageId} had delay {calc}");

                    var unaryRequest = new UnaryInfo()
                    {
                        Delay = calc.Duration(),
                        LengthOfData = timingValue.LengthOfData,
                        TypeOfData = timing.Value.TypeOfData,
                        DataContents = timing.Value.DataContents,
                    };

                    ClientMessageId actualDelayKeys = new ClientMessageId()
                    {
                        ClientId = timing.Key.ClientId,
                        MessageId = timing.Key.MessageId,
                    };


                    _requestResponseTimeStorage.AddToDictionary(_requestResponseTimeStorage._ActualDelayCalculations, actualDelayKeys, unaryRequest);

                    clientRequests.Remove(timing.Key);
                    serverResponses.Remove(timing.Key);

                    Console.WriteLine($"THIS IS THE AMOUNT OF THINGS IN THE CLIENT REQUESTS {clientRequests.Count}");
                    Console.WriteLine($"THIS IS THE AMOUNT OF THINGS IN THE SERVER RESPONSES {serverResponses.Count}");
                }
            }
        }

    }
}
