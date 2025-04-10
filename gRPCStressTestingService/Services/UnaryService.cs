using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using DbManagerWorkerService.Repositories;
using Google.Protobuf.Collections;
using Grpc.Core;
using gRPCStressTestingService.DelayCalculations;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService.proto;
using Serilog;
using SharedCommonalities.ObjectMapping;
using SharedCommonalities.Storage;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.UsefulFeatures;

namespace gRPCStressTestingService.Services
{
    public class UnaryService  : IUnaryService
    {
        private readonly RequestResponseTimeStorage _timeStorage;
        private readonly ClientStorage _storage;
        private readonly ObjectCreation _objectCreation;
        private readonly delayCalcRepo _delayCalcRepo;
        private readonly DelayCalculation _delayCalculations;
        private readonly DatabaseTransportationService _dbTransportationService;
        private readonly IClientInstanceRepo _clientInstanceRepo;
        private readonly ThroughputStorage _throughputStorage;
        public UnaryService(ClientStorage storage, RequestResponseTimeStorage timeStorage, ObjectCreation objectCreation, 
            delayCalcRepo delayCalcRepo, DelayCalculation delayCalculations, DatabaseTransportationService databaseTransportationService,
            IClientInstanceRepo clientInstanceRepo, ThroughputStorage throughputStorage)
        {
            _storage = storage;
            _timeStorage = timeStorage;
            _objectCreation = objectCreation;
            _delayCalcRepo = delayCalcRepo;
            _delayCalculations = delayCalculations;
            _dbTransportationService = databaseTransportationService;
            _clientInstanceRepo = clientInstanceRepo;
            _throughputStorage = throughputStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<DataResponse> UnaryResponse(DataRequest request, ServerCallContext context)
        {
            string preciseTime = GetPreciseTimeNow();

            string? typeOfDataFromMetaData = context.RequestHeaders.GetValue("request-type");
            string? numOfOpenChannels = context.RequestHeaders.GetValue("open-channels");
            string? numOfActiveClients = context.RequestHeaders.GetValue("active-clients");
            string? dataIterations = context.RequestHeaders.GetValue("data-iterations");
            string? dataContentSize = context.RequestHeaders.GetValue("data-content-size");
            
            Log.Information($"data content size {dataContentSize}");
            Log.Information($"Unary request message ID : {request.RequestId}");
            Log.Information($"this is the unary client request client count : {Settings.GetNumberOfActiveClients()}");

            ClientInstance getClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(Guid.Parse(request.ClientUnique));

            DataResponse dataReturn = new DataResponse()
            {
                ConnectionAlive = false,
                RequestId = request.RequestId,
                RequestType = request.RequestType,
                ResponseTimestamp = preciseTime, 
                ClientUnique = request.ClientUnique,  
            };

            _throughputStorage.IncrementSingleUnaryThroughput();

            ClientDetails clientDetails = _objectCreation.MappingToClientDetails(Guid.Parse(request.RequestId), 0, true, null,Guid.Parse( request.ClientUnique), request.DataContentSize);
            
            if (!_storage.Clients.ContainsKey(Guid.Parse(request.ClientUnique)))
            {
                ClientActivity clientActivity = new ClientActivity();

                clientActivity.AddToClientActivities(clientDetails);

                Log.Information($"Client ID : {request.ClientUnique} handles unary message -> {request.RequestId}");

                _storage.AddToDictionary(_storage.Clients, Guid.Parse(request.ClientUnique), clientActivity);
            }
            else
            {
                ClientActivity existingClientActivity = _storage.Clients[Guid.Parse(request.ClientUnique)];

                Log.Information($"Client ID : {request.ClientUnique} handles unary message -> {request.RequestId}");

                existingClientActivity.AddToClientActivities(clientDetails);
            }

            Settings.SetNumberOfActiveChannels(Convert.ToInt32(numOfOpenChannels));
            Settings.SetNumberOfActiveClients(Convert.ToInt32(numOfActiveClients));

            if(request.RequestId == string.Empty || request.RequestTimestamp == string.Empty)
            {
                Log.Warning($"The guid ({request.RequestId}) or the response time ({request.RequestTimestamp}) from the meta data was null");
            }

            UnaryInfo requestUnaryInfo = MapToRequest(Convert.ToDateTime(request.RequestTimestamp), typeOfDataFromMetaData, Convert.ToInt32(dataIterations), request.DataContent, 
                typeOfDataFromMetaData, null, request.DataContentSize, getClientInstance, dataIterations);

            UnaryInfo responseUnaryInfo = MapToResponse(Convert.ToDateTime(dataReturn.ResponseTimestamp), typeOfDataFromMetaData, Convert.ToInt32(dataIterations), request.DataContent, 
                typeOfDataFromMetaData, null, request.DataContentSize, getClientInstance, dataIterations);

            ClientMessageId requestAndResponseKeys = CreateClientMessageId(dataReturn.ClientUnique, request.RequestId);

            _timeStorage.AddToConcurrentDict(_timeStorage._ClientRequestTiming, requestAndResponseKeys, requestUnaryInfo);

            _timeStorage.AddToConcurrentDict(_timeStorage._ServerResponseTiming, requestAndResponseKeys, responseUnaryInfo );

            Console.WriteLine($"Client Id : {request.ClientUnique} handles message with ID : {request.RequestId}");

            Log.Information($"This is the request send time for the unary request : {requestUnaryInfo.TimeOfRequest} -> {request.RequestTimestamp}");
            Log.Information($"This is the server response time for the unary request : {responseUnaryInfo.TimeOfRequest} -> {preciseTime}");
          
            await _delayCalculations.CalculatingDelay(requestAndResponseKeys, requestAndResponseKeys);

            await _dbTransportationService.AddingDelayToDb();

            return dataReturn;
               
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<BatchDataResponse> BatchUnaryResponse(BatchDataRequest request, ServerCallContext context)
        {
            string preciseTime = GetPreciseTimeNow();

            string? batchTimestampFromMetaData = context.RequestHeaders.GetValue("batch-request-timestamp");
            string? typeOfDataFromMetaData = context.RequestHeaders.GetValue("request-type");
            int batchFromMetaData = Convert.ToInt32(context.RequestHeaders.GetValue("batch-request-count"));
            int numOfActiveClients = Convert.ToInt32(context.RequestHeaders.GetValue("active-clients"));
            string? requestIterations = context.RequestHeaders.GetValue("batch-iteration");

            List<ClientDetails> clientDetailsList = IteratingBatchToClientDetails(request.BatchDataRequest_);

            ClientDetails firstRequestElement = clientDetailsList[0];

            Guid clientUnique = firstRequestElement.ClientUnique;

            Guid batchRequestId = firstRequestElement.messageId;

            string requestContent = firstRequestElement.DataContent;

            string dataContentSize = firstRequestElement.DataContentSize;

            ClientInstance getClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(clientUnique);

            Log.Information($"Client ID: {clientUnique}");
            Log.Information($"The batch request ID : {batchRequestId}");
            Log.Information($"this is the batch client request client count -> {Settings.GetNumberOfActiveClients()}");

            BatchDataResponse batchDataResponse = new BatchDataResponse()
            {
                ClientUnique = clientUnique.ToString(),
                BatchRequestId = batchRequestId.ToString(),
                NumberOfRequestsInBatch = batchFromMetaData,
                ResponseTimestamp = preciseTime,
                RequestType = typeOfDataFromMetaData
            };

            if (!_storage.Clients.ContainsKey(clientUnique))
            {
                ClientActivity newClientActivity = new ClientActivity();

                newClientActivity.AddBatchToClientActivities(clientDetailsList);

                _storage.AddToDictionary(_storage.Clients, clientUnique, newClientActivity);
            }
            else
            {
                KeyValuePair<Guid, ClientActivity> retrieveExisting = _storage.Clients.FirstOrDefault(entry => entry.Key == clientUnique);

                retrieveExisting.Value.AddBatchToClientActivities(clientDetailsList);
            }

            UnaryInfo responseUnaryInfo = MapToResponse(Convert.ToDateTime(batchDataResponse.ResponseTimestamp), typeOfDataFromMetaData, batchFromMetaData, requestContent, 
                typeOfDataFromMetaData, batchRequestId.ToString(), dataContentSize, getClientInstance, requestIterations,  null);

            UnaryInfo requestUnaryInfo = MapToRequest(Convert.ToDateTime(batchTimestampFromMetaData), typeOfDataFromMetaData, batchFromMetaData, requestContent, 
                typeOfDataFromMetaData, batchRequestId.ToString(), dataContentSize, getClientInstance, requestIterations,  null);

            Log.Information($"This is the data content size : {responseUnaryInfo.DataContentSize}");
            Log.Information($"THIS IS THE DATA CONTENT : {responseUnaryInfo.LengthOfData}");

            ClientMessageId requestAndResponseKeys = CreateClientMessageId(clientUnique.ToString(), batchRequestId.ToString());

            _timeStorage.AddToConcurrentDictLock(_timeStorage._ClientRequestTiming, requestAndResponseKeys, requestUnaryInfo);

            _timeStorage.AddToConcurrentDictLock(_timeStorage._ServerResponseTiming, requestAndResponseKeys, responseUnaryInfo);

            Log.Information($"This is the request send time for the unary batch request : {requestUnaryInfo.TimeOfRequest} -> {batchTimestampFromMetaData}");
            Log.Information($"This is the server response time for the unary batch request : {responseUnaryInfo.TimeOfRequest} -> {preciseTime}");

            await _delayCalculations.CalculatingDelay(requestAndResponseKeys, requestAndResponseKeys);

            await _dbTransportationService.AddingDelayToDb();

            return batchDataResponse;
        }


        /// <summary>
        /// Gets the precise time of the current datetime 
        /// </summary>
        /// <returns></returns>
        private string GetPreciseTimeNow()
        {
            DateTime now = DateTime.UtcNow;
            long ticks = now.Ticks;

            string precisetime = now.ToString("HH:mm:ss.ffffff");

            return precisetime;

        }

        private UnaryInfo MapToRequest(DateTime? timeOfRequest, string? typeOfData, int lengthOfData, string? dataContent, string requestType, 
            string? batchRequestId, string dataContentSize, object clientInstance, string dataIterations,  TimeSpan? delay = null )
        {
           return _objectCreation.MappingToUnaryInfo(timeOfRequest, delay, typeOfData, lengthOfData, dataContent, requestType, batchRequestId, 
               dataContentSize, clientInstance, dataIterations);
        }

        private UnaryInfo MapToResponse(DateTime? timeOfRequest, string? typeOfData, int lengthOfData, string? dataContent, string requestType, 
            string? batchRequestId, string dataContentSize, object clientInstance, string dataIterations, TimeSpan? delay = null)
        {
            return _objectCreation.MappingToUnaryInfo(timeOfRequest, delay, typeOfData, lengthOfData, dataContent, requestType, batchRequestId, 
                dataContentSize, clientInstance, dataIterations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchRequestData"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<ClientDetails> IteratingBatchToClientDetails(RepeatedField<BatchDataRequestDetails> batchRequestData)
        {
            List<ClientDetails> clientDetailsList = new List<ClientDetails>();

            foreach(BatchDataRequestDetails details in batchRequestData)
            {

                _throughputStorage.IncrementBatchUnaryThroughput();

                Log.Information($"Client ID : {details.ClientUnique} with overarching ID { details.BatchRequestId} handles batch messages -> {details.RequestId}");

                int lengthOfData = LengthOfBatchRequest(details);

                string dataContent = details.DataContent;

                string clientUnique = details.ClientUnique;

                string dataContentsSize = details.DataContentSize;

                ClientDetails clientDetails = _objectCreation.MappingToClientDetails(Guid.Parse(details.RequestId), lengthOfData, details.ConnectionAlive, 
                    dataContent, Guid.Parse(details.ClientUnique), dataContentsSize);

                clientDetailsList.Add(clientDetails);
            }

            return clientDetailsList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchDataRequestDetails"></param>
        /// <returns></returns>
        private int LengthOfBatchRequest(BatchDataRequestDetails batchDataRequestDetails)
        {
            if(batchDataRequestDetails.DataSize == string.Empty)
            {
                Console.WriteLine($"There was no string data passed along with this request");
            }

            return batchDataRequestDetails.DataSize.Length;
        }

        private ClientMessageId CreateClientMessageId(string clientId, string messageId)
        {
            ClientMessageId clientMessageId = new ClientMessageId
            {
                ClientId = clientId,
                MessageId = messageId
            };

            return clientMessageId;
        }
    }
}
