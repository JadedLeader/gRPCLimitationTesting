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

            Log.Information($"Unary request message ID : {@guidFromMetaData}");
            Log.Information($"this is the unary client request client count : {Settings.GetNumberOfActiveClients()}");

            string preciseTime = GetPreciseTimeNow();

            var dataReturn = new DataResponse()
            {
                ConnectionAlive = false,
                RequestId = overarchingId,
                RequestType = request.RequestType,
                ResponseTimestamp = preciseTime
            };

            var clientDetails = _objectCreation.MappingToClientDetails(Guid.Parse(guidFromMetaData), 0, true);
            
            if (!_storage.Clients.ContainsKey(Guid.Parse(overarchingId)))
            {
                var clientActivity = new ClientActivity();

                clientActivity.AddToClientActivities(clientDetails);

                Log.Information($"Client ID : {overarchingId} handles unary message -> {guidFromMetaData}");

                _storage.AddToDictionary(_storage.Clients, Guid.Parse(overarchingId), clientActivity);
            }
            else
            {
                var existingClientActivity = _storage.Clients[Guid.Parse(overarchingId)];

                Log.Information($"Client ID : {overarchingId} handles unary message -> {guidFromMetaData}");

                existingClientActivity.AddToClientActivities(clientDetails);
            }

            Settings.SetNumberOfActiveChannels(Convert.ToInt32(numOfOpenChannels));
            Settings.SetNumberOfActiveClients(Convert.ToInt32(numOfActiveClients));

            if(guidFromMetaData == string.Empty || responseTimeFromMetaData == string.Empty)
            {
                Log.Warning($"The guid ({@guidFromMetaData}) or the response time ({@responseTimeFromMetaData}) from the meta data was null");
            }

            var requestUnaryInfo = MapToRequest(Convert.ToDateTime(responseTimeFromMetaData), typeOfDataFromMetaData, LengthOfRequest(request));

            var responseUnaryInfo = MapToResponse(Convert.ToDateTime(dataReturn.ResponseTimestamp), typeOfDataFromMetaData, LengthOfRequest(request));
            
            _timeStorage.AddToDictionary(_timeStorage._ClientRequestTiming, guidFromMetaData, requestUnaryInfo);

            _timeStorage.AddToDictionary(_timeStorage._ServerResponseTiming, guidFromMetaData, responseUnaryInfo);

            Console.WriteLine($"client storage guid {guidFromMetaData} SERVER GUID {dataReturn.RequestId}");

            Log.Information($"This is the request send time for the unary request : {requestUnaryInfo.TimeOfRequest} -> {responseTimeFromMetaData}");
            Log.Information($"This is the server response time for the unary request : {responseUnaryInfo.TimeOfRequest} -> {preciseTime}");

            return dataReturn;
               
        }

        public async Task<BatchDataResponse> BatchUnaryResponse(BatchDataRequest request, ServerCallContext context)
        {
            var batchIdFromMetaData = context.RequestHeaders.GetValue("batch-request-id");
            var batchTimestampFromMetaData = context.RequestHeaders.GetValue("batch-request-timestamp");
            var typeOfDataFromMetaData = context.RequestHeaders.GetValue("request-type");
            var batchFromMetaData = Convert.ToInt32(context.RequestHeaders.GetValue("batch-request-count"));
            var numOfActiveClients = Convert.ToInt32(context.RequestHeaders.GetValue("active-clients"));

            Log.Information($"Batch request ID {batchIdFromMetaData}");
            Log.Information($"this is the batch client request client count -> {Settings.GetNumberOfActiveClients()}");

            List<ClientDetails> clientDetailsList = IteratinBatchToClientDetails(request.BatchDataRequest_, batchIdFromMetaData);

            Settings.SetNumberOfActiveClients(numOfActiveClients);

            string preciseTime = GetPreciseTimeNow();

            var batchDataResponse = new BatchDataResponse()
            {
                BatchResponseId = batchIdFromMetaData,
                NumberOfRequestsInBatch = batchFromMetaData,
                ResponseTimestamp = preciseTime,
                RequestType = typeOfDataFromMetaData,
            };


            if(!_storage.Clients.ContainsKey(Guid.Parse(batchIdFromMetaData)))
            {
                //if it doesn't contain one we're going to add it to the client storage

                ClientActivity clientActivity = new ClientActivity();

                clientActivity.AddBatchToClientActivities(clientDetailsList);

                _storage.AddToDictionary(_storage.Clients, Guid.Parse(batchDataResponse.BatchResponseId), clientActivity);
            }
            else
            {
                //if it does contain one we just want to add it to the client storage value

                var getDictRecordViaGuid = _storage.Clients[Guid.Parse(batchIdFromMetaData)];

                getDictRecordViaGuid.AddBatchToClientActivities(clientDetailsList);
            }

            var responseUnaryInfo = MapToResponse(Convert.ToDateTime(batchDataResponse.ResponseTimestamp), typeOfDataFromMetaData, batchDataResponse.NumberOfRequestsInBatch);

            var requestUnaryInfo = MapToRequest(Convert.ToDateTime(batchTimestampFromMetaData), typeOfDataFromMetaData, batchFromMetaData);

            _timeStorage.AddToDictionary(_timeStorage._ClientRequestTiming, batchIdFromMetaData, requestUnaryInfo);

            _timeStorage.AddToDictionary(_timeStorage._ServerResponseTiming, batchDataResponse.BatchResponseId, responseUnaryInfo);

            Log.Information($"This is the request send time for the unary batch request : {requestUnaryInfo.TimeOfRequest} -> {batchTimestampFromMetaData}");
            Log.Information($"This is the server response time for the unary batch request : {responseUnaryInfo.TimeOfRequest} -> {preciseTime}");

            return batchDataResponse;
        }

        private string GetPreciseTimeNow()
        {
            var now = DateTime.UtcNow;
            long ticks = now.Ticks;

            string precisetime = now.ToString("HH:mm:ss.ffffff");

            return precisetime;

        }

        private UnaryInfo MapToRequest(DateTime? timeOfRequest, string? typeOfData, int? lengthOfData, TimeSpan? delay = null)
        {
           return _objectCreation.MappingToUnaryInfo(timeOfRequest, delay, typeOfData, lengthOfData);
        }

        private UnaryInfo MapToResponse(DateTime? timeOfRequest, string? typeOfData, int? lengthOfData, TimeSpan? delay = null)
        {
            return _objectCreation.MappingToUnaryInfo(timeOfRequest, delay, typeOfData, lengthOfData);
        }

        private List<ClientDetails> IteratinBatchToClientDetails(RepeatedField<DataRequest> batchRequestData, string id)
        {
            var clientDetailsList = new List<ClientDetails>();

            foreach(var details in batchRequestData)
            {
                Log.Information($"Client ID : {id} handles batch messages -> {details.RequestId} : {details.ConnectionAlive}");

                var lengthOfData = LengthOfRequest(details);

                var clientDetails = _objectCreation.MappingToClientDetails(Guid.Parse(details.RequestId), lengthOfData, details.ConnectionAlive);

                clientDetailsList.Add(clientDetails);
            }

            return clientDetailsList;
        }

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
