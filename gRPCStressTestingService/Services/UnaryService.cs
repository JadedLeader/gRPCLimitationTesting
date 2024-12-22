using Google.Protobuf.Collections;
using Grpc.Core;
using gRPCStressTestingService;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.proto;
using Microsoft.VisualBasic;
using Serilog;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.ObjectMapping;
using SharedCommonalities.ReturnModels;
using SharedCommonalities.Storage;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.UsefulFeatures;
using System.Data;
using System.Xml;

namespace gRPCStressTestingService.Services
{
    public class UnaryService  : IUnaryService
    {


        private readonly RequestResponseTimeStorage _timeStorage;
        private readonly ClientStorage _storage;
        private readonly ObjectCreation _objectCreation;

        public UnaryService(ClientStorage storage, RequestResponseTimeStorage timeStorage, ObjectCreation objectCreation)
        {
            _storage = storage;
            _timeStorage = timeStorage;
            _objectCreation = objectCreation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<DataResponse> UnaryResponse(DataRequest request, ServerCallContext context)
        {
            string? overarchingId = context.RequestHeaders.GetValue("overarching-id");
            string? guidFromMetaData = context.RequestHeaders.GetValue("request-id");
            string? responseTimeFromMetaData = context.RequestHeaders.GetValue("timestamp");
            string? typeOfDataFromMetaData = context.RequestHeaders.GetValue("request-type");
            string? numOfOpenChannels = context.RequestHeaders.GetValue("open-channels");
            string? numOfActiveClients = context.RequestHeaders.GetValue("active-clients");
            string? client = context.RequestHeaders.GetValue("client");
            string? clientUnique = context.RequestHeaders.GetValue("client-identifier");
            string? dataSize = context.RequestHeaders.GetValue("data-size");
            string? dataIterations = context.RequestHeaders.GetValue("data-iterations");

            Log.Information($"Unary request message ID : {@guidFromMetaData}");
            Log.Information($"this is the unary client request client count : {Settings.GetNumberOfActiveClients()}");

            string preciseTime = GetPreciseTimeNow();

            DataResponse dataReturn = new DataResponse()
            {
                ConnectionAlive = false,
                RequestId = overarchingId,
                RequestType = request.RequestType,
                ResponseTimestamp = preciseTime
            };

            ClientDetails clientDetails = _objectCreation.MappingToClientDetails(Guid.Parse(guidFromMetaData), 0, true, null);
            
            if (!_storage.Clients.ContainsKey(Guid.Parse(overarchingId)))
            {
                ClientActivity clientActivity = new ClientActivity();

                clientActivity.AddToClientActivities(clientDetails);

                Log.Information($"Client ID : {overarchingId} handles unary message -> {guidFromMetaData}");

                _storage.AddToDictionary(_storage.Clients, Guid.Parse(overarchingId), clientActivity);
            }
            else
            {
                ClientActivity existingClientActivity = _storage.Clients[Guid.Parse(overarchingId)];

                Log.Information($"Client ID : {overarchingId} handles unary message -> {guidFromMetaData}");

                existingClientActivity.AddToClientActivities(clientDetails);
            }

            Settings.SetNumberOfActiveChannels(Convert.ToInt32(numOfOpenChannels));
            Settings.SetNumberOfActiveClients(Convert.ToInt32(numOfActiveClients));

            if(guidFromMetaData == string.Empty || responseTimeFromMetaData == string.Empty)
            {
                Log.Warning($"The guid ({@guidFromMetaData}) or the response time ({@responseTimeFromMetaData}) from the meta data was null");
            }

            UnaryInfo requestUnaryInfo = MapToRequest(Convert.ToDateTime(responseTimeFromMetaData), typeOfDataFromMetaData, Convert.ToInt32(dataIterations), request.DataContent);

            UnaryInfo responseUnaryInfo = MapToResponse(Convert.ToDateTime(dataReturn.ResponseTimestamp), typeOfDataFromMetaData, Convert.ToInt32(dataIterations), request.DataContent);

            ClientMessageId requestKeys = new ClientMessageId()
            {
                ClientId = dataReturn.RequestId,
                MessageId = guidFromMetaData,
            };

            ClientMessageId responseKeys = new ClientMessageId()
            {
                ClientId = dataReturn.RequestId,
                MessageId = guidFromMetaData,
            };
            
            _timeStorage.AddToDictionary(_timeStorage._ClientRequestTiming, requestKeys, requestUnaryInfo);

            _timeStorage.AddToDictionary(_timeStorage._ServerResponseTiming, responseKeys, responseUnaryInfo);

            Console.WriteLine($"Client Id : {overarchingId} handles message with ID : {guidFromMetaData}");

            Log.Information($"This is the request send time for the unary request : {requestUnaryInfo.TimeOfRequest} -> {responseTimeFromMetaData}");
            Log.Information($"This is the server response time for the unary request : {responseUnaryInfo.TimeOfRequest} -> {preciseTime}");

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
            string? batchIterations = context.RequestHeaders.GetValue("batch-iterations");
            string? overachingBatchId = context.RequestHeaders.GetValue("overarching-id");
            string? batchIdFromMetaData = context.RequestHeaders.GetValue("batch-request-id");
            string? batchTimestampFromMetaData = context.RequestHeaders.GetValue("batch-request-timestamp");
            string? typeOfDataFromMetaData = context.RequestHeaders.GetValue("request-type");
            int batchFromMetaData = Convert.ToInt32(context.RequestHeaders.GetValue("batch-request-count"));
            int numOfActiveClients = Convert.ToInt32(context.RequestHeaders.GetValue("active-clients"));

            Log.Information($"Client ID: {overachingBatchId}");
            Log.Information($"The batch request ID : {batchIdFromMetaData}");
            Log.Information($"this is the batch client request client count -> {Settings.GetNumberOfActiveClients()}");

            List<ClientDetails> clientDetailsList = IteratingBatchToClientDetails(request.BatchDataRequest_, batchIdFromMetaData);

            List<string> requestDataContent = clientDetailsList.Select(entry => entry.DataContent).ToList();

            string requestContent = "";

            foreach(string unaryRequestItem in requestDataContent)
            {
                requestContent = unaryRequestItem;
            }

            Settings.SetNumberOfActiveClients(numOfActiveClients);

            string preciseTime = GetPreciseTimeNow();

            BatchDataResponse batchDataResponse = new BatchDataResponse()
            {
                BatchResponseId = batchIdFromMetaData,
                NumberOfRequestsInBatch = batchFromMetaData,
                ResponseTimestamp = preciseTime,
                RequestType = typeOfDataFromMetaData,
            };

            if(!_storage.Clients.ContainsKey(Guid.Parse(overachingBatchId)))
            {
                ClientActivity newClientActivity = new ClientActivity();

                newClientActivity.AddBatchToClientActivities(clientDetailsList);

                _storage.AddToDictionary(_storage.Clients, Guid.Parse(overachingBatchId), newClientActivity);
            }
            else
            {
                KeyValuePair<Guid, ClientActivity> retrieveExisting = _storage.Clients.FirstOrDefault(entry => entry.Key == Guid.Parse(overachingBatchId));

                retrieveExisting.Value.AddBatchToClientActivities(clientDetailsList);
            }

            UnaryInfo responseUnaryInfo = MapToResponse(Convert.ToDateTime(batchDataResponse.ResponseTimestamp), typeOfDataFromMetaData, Convert.ToInt32(batchDataResponse.NumberOfRequestsInBatch), requestContent);

            UnaryInfo requestUnaryInfo = MapToRequest(Convert.ToDateTime(batchTimestampFromMetaData), typeOfDataFromMetaData, batchFromMetaData, requestContent);

            Log.Information($"THIS IS THE DATA CONTENT : {responseUnaryInfo.DataContents}");
            Log.Information($"THIS IS THE DATA CONTENT : {responseUnaryInfo.LengthOfData}");

            ClientMessageId requestKeys = new ClientMessageId()
            {
                ClientId = overachingBatchId, 
                MessageId = batchIdFromMetaData,
            };

            ClientMessageId responseKeys = new ClientMessageId()
            {
                ClientId = overachingBatchId, 
                MessageId = batchIdFromMetaData,
            };

            _timeStorage.AddToDictionary(_timeStorage._ClientRequestTiming, requestKeys, requestUnaryInfo);

            _timeStorage.AddToDictionary(_timeStorage._ServerResponseTiming, responseKeys, responseUnaryInfo);

            Log.Information($"This is the request send time for the unary batch request : {requestUnaryInfo.TimeOfRequest} -> {batchTimestampFromMetaData}");
            Log.Information($"This is the server response time for the unary batch request : {responseUnaryInfo.TimeOfRequest} -> {preciseTime}");

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

        private UnaryInfo MapToRequest(DateTime? timeOfRequest, string? typeOfData, int? lengthOfData, string? dataContent,  TimeSpan? delay = null )
        {
           return _objectCreation.MappingToUnaryInfo(timeOfRequest, delay, typeOfData, lengthOfData, dataContent);
        }

        private UnaryInfo MapToResponse(DateTime? timeOfRequest, string? typeOfData, int? lengthOfData, string? dataContent, TimeSpan? delay = null)
        {
            return _objectCreation.MappingToUnaryInfo(timeOfRequest, delay, typeOfData, lengthOfData, dataContent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchRequestData"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<ClientDetails> IteratingBatchToClientDetails(RepeatedField<BatchDataRequestDetails> batchRequestData, string id)
        {
            List<ClientDetails> clientDetailsList = new List<ClientDetails>();

            foreach(BatchDataRequestDetails details in batchRequestData)
            {
                Log.Information($"Client ID : {details.OverarchingRequestId} handles batch messages -> {details.RequestId}");

                int lengthOfData = LengthOfBatchRequest(details);

                string dataContent = details.DataContent;

                ClientDetails clientDetails = _objectCreation.MappingToClientDetails(Guid.Parse(details.RequestId), lengthOfData, details.ConnectionAlive, dataContent);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRequest"></param>
        /// <returns></returns>
        private int LengthOfRequest(DataRequest dataRequest)
        {

            if(dataRequest.DataSize == string.Empty)
            {
                Console.WriteLine($"There was no string data passed along with this request");
            }

            return dataRequest.DataSize.Length;
        }

        

    }
}
