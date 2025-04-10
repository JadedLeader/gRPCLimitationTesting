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
                      
                        UnaryInfo delayResult = CreateUnaryInfo(calc.Duration(), serverTiming.LengthOfData, clientTiming.TypeOfData, clientTiming.DataContents, clientTiming.BatchRequestId,
                        clientTiming.RequestType, clientTiming.TimeOfRequest, clientTiming.DataContentSize, clientTiming.ClientInstance, clientTiming.DataIterations);

                        _timeStorage.AddToConcurrentDictLock(_timeStorage._ActualDelayCalculations, requestKeys, delayResult);
                    }
                }

                else if (clientTiming.RequestType == "Unary")
                {
                  
                    UnaryInfo delayResult = CreateUnaryInfo(calc.Duration(), serverTiming.LengthOfData, clientTiming.TypeOfData, clientTiming.DataContents, null,
                       clientTiming.RequestType, clientTiming.TimeOfRequest, clientTiming.DataContentSize, clientTiming.ClientInstance, clientTiming.DataIterations);

                    _timeStorage.AddToConcurrentDictLock(_timeStorage._ActualDelayCalculations, requestKeys, delayResult);
                }

                else if(clientTiming.RequestType == "Streaming")
                {
                   
                    UnaryInfo delayResult = CreateUnaryInfo(calc.Duration(), serverTiming.LengthOfData, clientTiming.TypeOfData, clientTiming.DataContents, null,
                        clientTiming.RequestType, clientTiming.TimeOfRequest, clientTiming.DataContentSize, clientTiming.ClientInstance, clientTiming.DataIterations);

                    _timeStorage.AddToConcurrentDictLock(_timeStorage._ActualDelayCalculations, requestKeys, delayResult);
                }

                else if(clientTiming.RequestType == "StreamingBatch")
                {
                    
                    UnaryInfo delayResult = CreateUnaryInfo(calc.Duration(), serverTiming.LengthOfData, clientTiming.TypeOfData, clientTiming.DataContents, clientTiming.BatchRequestId,
                        clientTiming.RequestType, clientTiming.TimeOfRequest, clientTiming.DataContentSize, clientTiming.ClientInstance, clientTiming.DataIterations);

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

        private UnaryInfo CreateUnaryInfo(TimeSpan? Delay, int lengthOfData, string? typeOfData, string dataContents, string? batchRequestId, string requestType, 
            DateTime? timeOfRequest, string dataContentSize, object? clientInstance, string dataIterations)
        {
            UnaryInfo unaryInfo = new UnaryInfo
            {
                Delay = Delay,
                LengthOfData = lengthOfData,
                TypeOfData = typeOfData,
                DataContents = dataContents,
                BatchRequestId = batchRequestId,
                RequestType = requestType,
                TimeOfRequest = timeOfRequest,
                DataContentSize = dataContentSize,
                ClientInstance = clientInstance,
                DataIterations = dataIterations
            }; 

            return unaryInfo;
        }

    }
}
