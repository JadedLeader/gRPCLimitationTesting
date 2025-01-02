using Azure.Core;
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
           
            string? typeOfDataFromMetaData = context.RequestHeaders.GetValue("request-type");
            string? numOfOpenChannels = context.RequestHeaders.GetValue("open-channels");
            string? numOfActiveClients = context.RequestHeaders.GetValue("active-clients");
            string? dataIterations = context.RequestHeaders.GetValue("data-iterations");

            Log.Information($"Unary request message ID : {request.RequestId}");
            Log.Information($"this is the unary client request client count : {Settings.GetNumberOfActiveClients()}");

            string preciseTime = GetPreciseTimeNow();

            DataResponse dataReturn = new DataResponse()
            {
                ConnectionAlive = false,
                RequestId = request.RequestId,
                RequestType = request.RequestType,
                ResponseTimestamp = preciseTime, 
                ClientUnique = request.ClientUnique,
                
            };

            ClientDetails clientDetails = _objectCreation.MappingToClientDetails(Guid.Parse(request.RequestId), 0, true, null,Guid.Parse( request.ClientUnique));
            
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

            UnaryInfo requestUnaryInfo = MapToRequest(Convert.ToDateTime(request.RequestTimestamp), typeOfDataFromMetaData, Convert.ToInt32(dataIterations), request.DataContent);

            UnaryInfo responseUnaryInfo = MapToResponse(Convert.ToDateTime(dataReturn.ResponseTimestamp), typeOfDataFromMetaData, Convert.ToInt32(dataIterations), request.DataContent);

            ClientMessageId requestKeys = new ClientMessageId()
            {
                ClientId = dataReturn.ClientUnique,
                MessageId = request.RequestId,
            };

            ClientMessageId responseKeys = new ClientMessageId()
            {
                ClientId = dataReturn.ClientUnique,
                MessageId = request.RequestId,
            };
            
            _timeStorage.AddToDictionary(_timeStorage._ClientRequestTiming, requestKeys, requestUnaryInfo);

            _timeStorage.AddToDictionary(_timeStorage._ServerResponseTiming, responseKeys, responseUnaryInfo);

            Console.WriteLine($"Client Id : {request.ClientUnique} handles message with ID : {request.RequestId}");

            Log.Information($"This is the request send time for the unary request : {requestUnaryInfo.TimeOfRequest} -> {request.RequestTimestamp}");
            Log.Information($"This is the server response time for the unary request : {responseUnaryInfo.TimeOfRequest} -> {preciseTime}");

           var thing = _timeStorage.ReturnDictionary(_timeStorage._ClientRequestTiming);
           var thing1 = _timeStorage.ReturnDictionary(_timeStorage._ServerResponseTiming);
           var thing2 = _timeStorage.ReturnDictionary( _timeStorage._ActualDelayCalculations);

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
            
            string? batchTimestampFromMetaData = context.RequestHeaders.GetValue("batch-request-timestamp");
            string? typeOfDataFromMetaData = context.RequestHeaders.GetValue("request-type");
            int batchFromMetaData = Convert.ToInt32(context.RequestHeaders.GetValue("batch-request-count"));
            int numOfActiveClients = Convert.ToInt32(context.RequestHeaders.GetValue("active-clients"));

            List<ClientDetails> clientDetailsList = IteratingBatchToClientDetails(request.BatchDataRequest_);

            ClientDetails firstRequestElement = clientDetailsList[0];

            Guid clientUnique = firstRequestElement.ClientUnique;

            Guid batchRequestId = firstRequestElement.messageId;

            string requestContent = firstRequestElement.DataContent;

            Log.Information($"Client ID: {clientUnique}");
            Log.Information($"The batch request ID : {batchRequestId}");
            Log.Information($"this is the batch client request client count -> {Settings.GetNumberOfActiveClients()}");

            Settings.SetNumberOfActiveClients(numOfActiveClients);

            string preciseTime = GetPreciseTimeNow();

            BatchDataResponse batchDataResponse = new BatchDataResponse()
            {
                ClientUnique = clientUnique.ToString(),
                BatchRequestId = batchRequestId.ToString(),
                NumberOfRequestsInBatch = batchFromMetaData,
                ResponseTimestamp = preciseTime,
                RequestType = typeOfDataFromMetaData,
            };

            if(!_storage.Clients.ContainsKey(clientUnique))
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

            UnaryInfo responseUnaryInfo = MapToResponse(Convert.ToDateTime(batchDataResponse.ResponseTimestamp), typeOfDataFromMetaData, batchFromMetaData, requestContent);

            UnaryInfo requestUnaryInfo = MapToRequest(Convert.ToDateTime(batchTimestampFromMetaData), typeOfDataFromMetaData, batchFromMetaData, requestContent);

            Log.Information($"THIS IS THE DATA CONTENT : {responseUnaryInfo.DataContents}");
            Log.Information($"THIS IS THE DATA CONTENT : {responseUnaryInfo.LengthOfData}");

            ClientMessageId requestKeys = new ClientMessageId()
            {
                ClientId = clientUnique.ToString(), 
                MessageId = batchRequestId.ToString(),
            };

            ClientMessageId responseKeys = new ClientMessageId()
            {
                ClientId = clientUnique.ToString(), 
                MessageId = batchRequestId.ToString(),
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
        private List<ClientDetails> IteratingBatchToClientDetails(RepeatedField<BatchDataRequestDetails> batchRequestData)
        {
            List<ClientDetails> clientDetailsList = new List<ClientDetails>();

            foreach(BatchDataRequestDetails details in batchRequestData)
            {
                Log.Information($"Client ID : {details.ClientUnique} handles batch messages -> {details.RequestId}");

                int lengthOfData = LengthOfBatchRequest(details);

                string dataContent = details.DataContent;

                string clientUnique = details.ClientUnique;

                ClientDetails clientDetails = _objectCreation.MappingToClientDetails(Guid.Parse(details.RequestId), lengthOfData, details.ConnectionAlive, dataContent, Guid.Parse(details.ClientUnique));

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
