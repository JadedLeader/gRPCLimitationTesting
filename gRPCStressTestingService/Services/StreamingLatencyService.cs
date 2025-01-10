using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using DbManagerWorkerService.Repositories;
using Grpc.Core;
using gRPCStressTestingService.DelayCalculations;
using gRPCStressTestingService.Interfaces.Services;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.VisualBasic;
using Serilog;
using SharedCommonalities.Storage;
using SharedCommonalities.TimeStorage;
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

        public StreamingLatencyService(RequestResponseTimeStorage responseTimeStorage, delayCalcRepo delayCalcRepo, DelayCalculation delayCalculation,
            DatabaseTransportationService databaseTransportationService, IClientInstanceRepo clientInstanceRepo)
        {
            _responseTimeStorage = responseTimeStorage;
            _delayCalcRepo = delayCalcRepo;
            _delayCalculation = delayCalculation;
            _dbTransportationService = databaseTransportationService;
            _clientInstanceRepo = clientInstanceRepo;
        }


        public async Task StreamingBatchRequest(IAsyncStreamReader<StreamingBatchLatencyRequest> requestStream, IServerStreamWriter<StreamingBatchLatencyResponse> responseStream, ServerCallContext context)
        {
            throw new NotImplementedException();
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
            var preciseTime = GetPreciseTimeNow();

         
            await foreach(var request in requestStream.ReadAllAsync())
            {
                //receive every request from the stream, and then we'll add it to a list 

                Log.Information($"Received streaming request in stream, with client unique {request.ClientUnique} and message ID: {request.RequestId}");

                //when we read a request from the stream, we're going to want to adad it to the request response time dict

                ClientInstance findingClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(Guid.Parse(request.ClientUnique));

                ClientMessageId requestKeys = new ClientMessageId
                {
                    ClientId = request.ClientUnique,
                    MessageId = request.RequestId,
                };

                UnaryInfo streamingInfoRequest = new UnaryInfo
                {
                    TimeOfRequest = DateTime.Parse(request.RequestTimestamp),
                    DataContents = request.DataContent,
                    TypeOfData = request.RequestType,
                    RequestType = request.RequestType,
                    LengthOfData = 0,
                    BatchRequestId = null, 
                    Delay = null, 
                    DataContentSize = request.DataContentSize,
                    ClientInstance = findingClientInstance,

                };

                _responseTimeStorage.AddToConcurrentDictLock(_responseTimeStorage._ClientRequestTiming, requestKeys, streamingInfoRequest);

                var requestTime = _responseTimeStorage.ReturnConcurrentDictLock(_responseTimeStorage._ClientRequestTiming);

                Log.Information($"Current request time dict count: {requestTime.Count}");

                StreamingSingleLatencyResponse serverResponse = new StreamingSingleLatencyResponse
                {
                    ClientUnique = request.ClientUnique,
                    RequestId = request.RequestId,
                    ConnectionAlive = request.ConnectionAlive,
                    RequestType = request.RequestType,
                    RequestTimestamp = preciseTime,
                };

                ClientMessageId responseKeys = new ClientMessageId
                {
                    ClientId = request.ClientUnique,
                    MessageId = request.RequestId,
                };

                UnaryInfo streamingInfoResponse = new UnaryInfo
                {
                    TimeOfRequest = DateTime.Parse(serverResponse.RequestTimestamp),
                    DataContents = request.DataContent,
                    TypeOfData = request.RequestType,
                    RequestType = request.RequestType,
                    LengthOfData = 0,
                    BatchRequestId = null,
                    Delay = null, 
                    DataContentSize= request.DataContentSize,
                    ClientInstance = findingClientInstance,

                };

                _responseTimeStorage.AddToConcurrentDictLock(_responseTimeStorage._ServerResponseTiming, responseKeys, streamingInfoResponse);

                var responseTime = _responseTimeStorage.ReturnConcurrentDictLock(_responseTimeStorage._ServerResponseTiming);

                Log.Information($"Current repsonse time storage count: {responseTime.Count}");

                Log.Information($"Responding with request from stream, with client unique : {serverResponse.ClientUnique} and request ID : {serverResponse.RequestId}");

                await responseStream.WriteAsync(serverResponse);

                await _delayCalculation.CalculatingDelay(requestKeys, responseKeys);

                await _dbTransportationService.AddingDelayToDb();

            }

            

        }

       
        private string GetPreciseTimeNow()
        {
            DateTime now = DateTime.UtcNow;
            long ticks = now.Ticks;

            string precisetime = now.ToString("HH:mm:ss.ffffff");

            return precisetime;

        }

    }
}
