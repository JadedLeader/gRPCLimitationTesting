using Serilog;
using SharedCommonalities.TimeStorage;

namespace gRPCStressTestingService.DelayCalculations
{
    public class DelayCalculation
    {

        private readonly RequestResponseTimeStorage _timeStorage;

        public DelayCalculation(RequestResponseTimeStorage timeStorage)
        {
            _timeStorage = timeStorage;
        }


        public async Task CalculatingDelay(ClientMessageId requestKeys, ClientMessageId responseKeys)
        {
            Log.Information($"Calculating delay immediately after request.");
            var clientRequests = _timeStorage.ReturnConcurrentDictLock(_timeStorage._ClientRequestTiming);
            var serverResponses = _timeStorage.ReturnConcurrentDictLock(_timeStorage._ServerResponseTiming);

            if (clientRequests.TryGetValue(requestKeys, out var clientTiming) &&
                serverResponses.TryGetValue(responseKeys, out var serverTiming))
            {
                var calc = Convert.ToDateTime(serverTiming.TimeOfRequest.Value) - Convert.ToDateTime(clientTiming.TimeOfRequest.Value);

                Log.Information($"Client ID: {requestKeys.ClientId} with message ID {requestKeys.MessageId} had delay {calc}");

                if (clientTiming.RequestType == "BatchUnary")
                {
                    if (!_timeStorage._ActualDelayCalculations.ContainsKey(requestKeys))
                    {
                        var delayResult = new UnaryInfo
                        {
                            Delay = calc.Duration(),
                            LengthOfData = serverTiming.LengthOfData,
                            TypeOfData = clientTiming.TypeOfData,
                            DataContents = clientTiming.DataContents,
                            BatchRequestId = clientTiming.BatchRequestId,
                            RequestType = clientTiming.RequestType,
                            TimeOfRequest = clientTiming.TimeOfRequest,
                            DataContentSize = clientTiming.DataContentSize,
                        };

                        _timeStorage.AddToConcurrentDictLock(_timeStorage._ActualDelayCalculations, requestKeys, delayResult);
                    }
                }

                else if (clientTiming.RequestType == "Unary")
                {
                    var delayResult = new UnaryInfo
                    {
                        Delay = calc.Duration(),
                        LengthOfData = serverTiming.LengthOfData,
                        TypeOfData = clientTiming.TypeOfData,
                        DataContents = clientTiming.DataContents,
                        BatchRequestId = null,
                        RequestType = clientTiming.RequestType,
                        TimeOfRequest = clientTiming.TimeOfRequest,
                        DataContentSize = clientTiming.DataContentSize,
                    };

                    _timeStorage.AddToConcurrentDictLock(_timeStorage._ActualDelayCalculations, requestKeys, delayResult);
                }

                else if(clientTiming.RequestType == "Streaming")
                {
                    var delayResult = new UnaryInfo
                    {
                        Delay = calc.Duration(),
                        LengthOfData = serverTiming.LengthOfData,
                        TypeOfData = clientTiming.TypeOfData,
                        DataContents = clientTiming.DataContents,
                        BatchRequestId = null,
                        RequestType = clientTiming.RequestType,
                        TimeOfRequest = clientTiming.TimeOfRequest,
                        
                    };

                    _timeStorage.AddToConcurrentDictLock(_timeStorage._ActualDelayCalculations, requestKeys, delayResult);
                }

                _timeStorage.RemoveFromConcurrentDictLock(_timeStorage._ClientRequestTiming, requestKeys);
                _timeStorage.RemoveFromConcurrentDictLock(_timeStorage._ServerResponseTiming, responseKeys);
            }
            else
            {
                Log.Warning($"Could not find matching request and response for delay calculation.");
            }
        }

    }
}
