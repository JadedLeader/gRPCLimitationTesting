using Azure.Core;
using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using DbManagerWorkerService.Repositories;
using Grpc.Core;
using gRPCStressTestingService.DelayCalculations;
using gRPCStressTestingService.Interfaces.Services;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.VisualBasic;
using Serilog;
using SharedCommonalities.Storage;
using SharedCommonalities.TimeStorage;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
namespace gRPCStressTestingService.Services
{
    public class StreamingLatencyService : IStreamingLatencyService
    {
        private readonly RequestResponseTimeStorage _responseTimeStorage;
        private readonly IDelayCalcRepo _delayCalcRepo;
        private readonly DelayCalculation _delayCalculation;
        private readonly DatabaseTransportationService _dbTransportationService;
        private readonly IClientInstanceRepo _clientInstanceRepo;
        private readonly ThroughputStorage _throughputStorage;

        public StreamingLatencyService(RequestResponseTimeStorage responseTimeStorage, delayCalcRepo delayCalcRepo, DelayCalculation delayCalculation,
            DatabaseTransportationService databaseTransportationService, IClientInstanceRepo clientInstanceRepo, ThroughputStorage throughputStorage)
        {
            _responseTimeStorage = responseTimeStorage;
            _delayCalcRepo = delayCalcRepo;
            _delayCalculation = delayCalculation;
            _dbTransportationService = databaseTransportationService;
            _clientInstanceRepo = clientInstanceRepo;
            _throughputStorage = throughputStorage;
        }

        /// <summary>
        /// This method is in charge of responding with a single unary request in a streaming manner
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task StreamingSingleRequest(IAsyncStreamReader<StreamingSingleLatencyRequest> requestStream, IServerStreamWriter<StreamingSingleLatencyResponse> responseStream, ServerCallContext context)
        {
            string preciseTime = GetPreciseTimeNow();
            
            await foreach(var request in requestStream.ReadAllAsync())
            {
                
                Log.Information($"Received streaming request in stream, with client unique {request.ClientUnique} and message ID: {request.RequestId}");

                ClientInstance findingClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(Guid.Parse(request.ClientUnique));

                ClientMessageId keys = await CreateClientMessageId(request.ClientUnique, request.RequestId);
                
                UnaryInfo streamingInfoRequest = await CreateUnaryInfoStreamingRequest(DateTime.Parse(request.RequestTimestamp), request.RequestType, request.DataContent, 
                    request.RequestType, request.DataContentSize, findingClientInstance, "1", null); 

                _responseTimeStorage.AddToConcurrentDictLock(_responseTimeStorage._ClientRequestTiming, keys, streamingInfoRequest);

                ConcurrentDictionary<ClientMessageId, UnaryInfo> requestTime = _responseTimeStorage.ReturnConcurrentDictLock(_responseTimeStorage._ClientRequestTiming);

                Log.Information($"Current request time dict count: {requestTime.Count}");

                StreamingSingleLatencyResponse serverResponse = new StreamingSingleLatencyResponse
                {
                    ClientUnique = request.ClientUnique,
                    RequestId = request.RequestId,
                    ConnectionAlive = request.ConnectionAlive,
                    RequestType = request.RequestType,
                    RequestTimestamp = preciseTime,
                };

                _throughputStorage.IncrementSingleStreamingThroughput();

                UnaryInfo streamingInfoResponse = await CreateUnaryInfoStreamingRequest(DateTime.Parse(serverResponse.RequestTimestamp), request.RequestType, request.DataContent,
                    request.RequestType, request.DataContentSize, findingClientInstance, "1", null);
               
                _responseTimeStorage.AddToConcurrentDictLock(_responseTimeStorage._ServerResponseTiming, keys, streamingInfoResponse);

                ConcurrentDictionary<ClientMessageId, UnaryInfo> responseTime = _responseTimeStorage.ReturnConcurrentDictLock(_responseTimeStorage._ServerResponseTiming);

                Log.Information($"Current repsonse time storage count: {responseTime.Count}");

                Log.Information($"Responding with request from stream, with client unique : {serverResponse.ClientUnique} and request ID : {serverResponse.RequestId}");

                await responseStream.WriteAsync(serverResponse);

                await _delayCalculation.CalculatingDelay(keys, keys);

                await _dbTransportationService.AddingDelayToDb();

            }

        }


        /// <summary>
        /// We require this endpoint, as right now we want all these messages in the streaming request to share the same streaming client, we dont want to create a new instance 
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task StreamingManySingleRequest(IAsyncStreamReader<StreamingManySingleLatencyRequest> requestStream, IServerStreamWriter<StreamingManySingleLatencyResponse> responseStream, ServerCallContext context)
        {

            string preciseTime = GetPreciseTimeNow();

            string? dataIterations = context.RequestHeaders.GetValue("data-iterations");

            if(dataIterations == null)
            {
                Log.Warning($"Data iterations was null in the streaming many single requests endpoint");
            }

            await foreach(var request in requestStream.ReadAllAsync())
            {

                ClientInstance getClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(Guid.Parse(request.ClientUnique));

                if(getClientInstance == null)
                {
                    Log.Error($"There was no client instance for the streaming many single requests");

                    throw new RpcException(new Status(StatusCode.NotFound, $"Client instance could not be found for the streaming many single requests"));
                }

                ClientMessageId keys = await CreateClientMessageId(request.ClientUnique, request.RequestId);

                UnaryInfo requestInfo = await CreateUnaryInfoStreamingRequest(DateTime.Parse(request.RequestTimestamp), request.RequestType, request.DataContent, request.RequestType,
                    request.DataContentSize, getClientInstance, dataIterations, null);

                _responseTimeStorage.AddToConcurrentDictLock(_responseTimeStorage._ClientRequestTiming, keys, requestInfo);


                UnaryInfo responseInfo = await CreateUnaryInfoStreamingRequest(DateTime.Parse(preciseTime), request.RequestType, request.DataContent, request.RequestType,
                    request.DataContentSize, getClientInstance, dataIterations, null);

                _responseTimeStorage.AddToConcurrentDictLock(_responseTimeStorage._ServerResponseTiming, keys, responseInfo);

                StreamingManySingleLatencyResponse serverResponse = new StreamingManySingleLatencyResponse
                {
                    ClientUnique = request.ClientUnique,
                    RequestId = request.RequestId,
                    ConnectionAlive = true,
                    RequestTimestamp = preciseTime,
                    RequestType = request.RequestType,
                };

                _throughputStorage.IncrementSingleStreamingThroughput();

                await responseStream.WriteAsync(serverResponse);

                await _delayCalculation.CalculatingDelay(keys, keys);

                await _dbTransportationService.AddingDelayToDb();
            }

        }


        public async Task StreamingSingleBatchRequest(IAsyncStreamReader<StreamingBatchLatencyRequest> requestStream, IServerStreamWriter<StreamingBatchLatencyResponse> responseStream, ServerCallContext context)
        {

            var preciseTime = GetPreciseTimeNow();

            string? dataIterations = context.RequestHeaders.GetValue("data-iterations");

            if(dataIterations == null)
            {
                Log.Warning($"No data iterations were passed to the streaming single batch request");
            }

            await foreach(var request in requestStream.ReadAllAsync())
            {
                foreach(var requestPayload in request.StreamingBatchDataRequest)
                {

                    ClientMessageId keys = await CreateClientMessageId(requestPayload.ClientUnique, requestPayload.MessageId); 

                    ClientInstance gettingClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(Guid.Parse(requestPayload.ClientUnique));

                    UnaryInfo requestUnaryInfo = await CreateUnaryInfoStreamingRequest(DateTime.Parse(requestPayload.RequestTimestamp), requestPayload.RequestType, requestPayload.DataContent,
                        requestPayload.RequestType, requestPayload.DataContentSize, gettingClientInstance, dataIterations, requestPayload.BatchRequestId);

                    _responseTimeStorage.AddToConcurrentDictLock(_responseTimeStorage._ClientRequestTiming, keys, requestUnaryInfo);

                    UnaryInfo responseUnaryInfo = await CreateUnaryInfoStreamingRequest(DateTime.Parse(preciseTime), requestPayload.RequestType, requestPayload.DataContent,
                        requestPayload.RequestType, requestPayload.DataContentSize, gettingClientInstance, dataIterations, requestPayload.BatchRequestId);

                    _responseTimeStorage.AddToConcurrentDictLock(_responseTimeStorage._ServerResponseTiming, keys, responseUnaryInfo);

                    StreamingBatchLatencyResponse serverResponse = new StreamingBatchLatencyResponse
                    {
                        BatchRequestId = requestPayload.BatchRequestId,
                        ClientUnique = requestPayload.ClientUnique,
                        NumberOfRequestsInBatch = 0,
                        RequestType = requestPayload.RequestType,
                        ResponseTimestamp = preciseTime
                    };

                    _throughputStorage.IncrementBatchStreamingThroughput();

                    await responseStream.WriteAsync(serverResponse);

                    await _delayCalculation.CalculatingDelay(keys, keys);

                    await _dbTransportationService.AddingDelayToDb();

                }
            }
        }


        private string GetPreciseTimeNow()
        {
            DateTime now = DateTime.Now;
            long ticks = now.Ticks;

            string precisetime = now.ToString("HH:mm:ss.ffffff");

            return precisetime;

        }

        private async Task<ClientMessageId> CreateClientMessageId(string clientUnique, string messageId)
        {
            ClientMessageId keys = new ClientMessageId
            {
                ClientId = clientUnique,
                MessageId = messageId,
            };

            return keys;
        }

        private async Task<UnaryInfo> CreateUnaryInfoStreamingRequest(DateTime timeOfRequest, string typeOfData, string dataContents, string requestType, string dataContentSize,
            object clientInstance, string dataIterations, string? batchDataRequestId)
        {
            UnaryInfo info = new UnaryInfo
            {
                TimeOfRequest = timeOfRequest,
                TypeOfData = requestType,
                LengthOfData = 0,
                DataContents = dataContents,
                RequestType = requestType,
                DataContentSize = dataContentSize,
                BatchRequestId = batchDataRequestId,
                ClientInstance = clientInstance,
                DataIterations = dataIterations

            };

            return info;
        }

    }
}
