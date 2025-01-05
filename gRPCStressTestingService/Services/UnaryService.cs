using Azure.Core;
using ConfigurationStuff.DbModels;
using DbManagerWorkerService.Repositories;
using Google.Protobuf.Collections;
using Grpc.Core;
using gRPCStressTestingService;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.proto;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.VisualBasic;
using Serilog;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.ObjectMapping;
using SharedCommonalities.ReturnModels;
using SharedCommonalities.Storage;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.UsefulFeatures;
using System.Data;
using System.Runtime.CompilerServices;
using System.Xml;

namespace gRPCStressTestingService.Services
{
    public class UnaryService  : IUnaryService
    {


        private readonly RequestResponseTimeStorage _timeStorage;
        private readonly ClientStorage _storage;
        private readonly ObjectCreation _objectCreation;
        private readonly delayCalcRepo _delayCalcRepo;

        public UnaryService(ClientStorage storage, RequestResponseTimeStorage timeStorage, ObjectCreation objectCreation, delayCalcRepo delayCalcRepo)
        {
            _storage = storage;
            _timeStorage = timeStorage;
            _objectCreation = objectCreation;
            _delayCalcRepo = delayCalcRepo;
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

            UnaryInfo requestUnaryInfo = MapToRequest(Convert.ToDateTime(request.RequestTimestamp), typeOfDataFromMetaData, Convert.ToInt32(dataIterations), request.DataContent, typeOfDataFromMetaData, null);

            UnaryInfo responseUnaryInfo = MapToResponse(Convert.ToDateTime(dataReturn.ResponseTimestamp), typeOfDataFromMetaData, Convert.ToInt32(dataIterations), request.DataContent, typeOfDataFromMetaData, null);

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
            
            //_timeStorage.AddToDictionary(_timeStorage._ClientRequestTiming, requestKeys, requestUnaryInfo);

            _timeStorage.AddToConcurrentDict(_timeStorage._ClientRequestTiming, requestKeys, requestUnaryInfo);

            //_timeStorage.AddToDictionary(_timeStorage._ServerResponseTiming, responseKeys, responseUnaryInfo);

            _timeStorage.AddToConcurrentDict(_timeStorage._ServerResponseTiming, responseKeys, responseUnaryInfo );

            Console.WriteLine($"Client Id : {request.ClientUnique} handles message with ID : {request.RequestId}");

            Log.Information($"This is the request send time for the unary request : {requestUnaryInfo.TimeOfRequest} -> {request.RequestTimestamp}");
            Log.Information($"This is the server response time for the unary request : {responseUnaryInfo.TimeOfRequest} -> {preciseTime}");

           //var thing = _timeStorage.ReturnDictionary(_timeStorage._ClientRequestTiming);
           //var thing1 = _timeStorage.ReturnDictionary(_timeStorage._ServerResponseTiming);
           
            var clientRequests = _timeStorage.ReturnConcurrentDict(_timeStorage._ClientRequestTiming);
            var responseTimings = _timeStorage.ReturnConcurrentDict(_timeStorage._ServerResponseTiming);

            var thing2 = _timeStorage.ReturnConcurrentDict( _timeStorage._ActualDelayCalculations);

            await CalculatingDelay(requestKeys, responseKeys);

            await AddingDelayToDb();

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
                RequestType = typeOfDataFromMetaData
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

            UnaryInfo responseUnaryInfo = MapToResponse(Convert.ToDateTime(batchDataResponse.ResponseTimestamp), typeOfDataFromMetaData, batchFromMetaData, requestContent, typeOfDataFromMetaData, batchRequestId.ToString());

            UnaryInfo requestUnaryInfo = MapToRequest(Convert.ToDateTime(batchTimestampFromMetaData), typeOfDataFromMetaData, batchFromMetaData, requestContent, typeOfDataFromMetaData, batchRequestId.ToString());

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

            _timeStorage.AddToConcurrentDictLock(_timeStorage._ClientRequestTiming, requestKeys, requestUnaryInfo);

            _timeStorage.AddToConcurrentDictLock(_timeStorage._ServerResponseTiming, responseKeys, responseUnaryInfo);

            Log.Information($"This is the request send time for the unary batch request : {requestUnaryInfo.TimeOfRequest} -> {batchTimestampFromMetaData}");
            Log.Information($"This is the server response time for the unary batch request : {responseUnaryInfo.TimeOfRequest} -> {preciseTime}");

            var thing = _timeStorage.ReturnConcurrentDictLock(_timeStorage._ClientRequestTiming);
            var thing1 = _timeStorage.ReturnConcurrentDictLock(_timeStorage._ServerResponseTiming);
            var thing2 = _timeStorage.ReturnConcurrentDictLock(_timeStorage._ActualDelayCalculations);

            Log.Information($"Amount of things in client request timing {thing.Count}");
            Log.Information($"Amount of things in server response timing {thing1.Count}");

            await CalculatingDelay(requestKeys, responseKeys);

            await AddingDelayToDb();

            return batchDataResponse;
        }

        private async Task CalculatingDelay(ClientMessageId requestKeys, ClientMessageId responseKeys)
        {
            Log.Information($"Calculating delay immediately after request.");
            var clientRequests = _timeStorage.ReturnConcurrentDictLock(_timeStorage._ClientRequestTiming);
            var serverResponses = _timeStorage.ReturnConcurrentDictLock(_timeStorage._ServerResponseTiming);

            if (clientRequests.TryGetValue(requestKeys, out var clientTiming) &&
                serverResponses.TryGetValue(responseKeys, out var serverTiming))
            {
                var calc = Convert.ToDateTime(serverTiming.TimeOfRequest.Value)
                         - Convert.ToDateTime(clientTiming.TimeOfRequest.Value);

                Log.Information($"Client ID: {requestKeys.ClientId} with message ID {requestKeys.MessageId} had delay {calc}");

                // ✅ Check for Batch Request (shared IDs)
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
                        };

                        _timeStorage.AddToConcurrentDictLock(_timeStorage._ActualDelayCalculations, requestKeys, delayResult);
                    }
                }


                // ✅ Handling Single Requests (Unique IDs)
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
                    };

                    // Save single request calculation (since each has a unique ID)
                    _timeStorage.AddToConcurrentDictLock(_timeStorage._ActualDelayCalculations, requestKeys, delayResult);
                }

                // ✅ Remove from dictionaries after processing
                _timeStorage.RemoveFromConcurrentDictLock(_timeStorage._ClientRequestTiming, requestKeys);
                _timeStorage.RemoveFromConcurrentDictLock(_timeStorage._ServerResponseTiming, responseKeys);
            }
            else
            {
                Log.Warning($"Could not find matching request and response for delay calculation.");
            }
        }

        private async Task AddingDelayToDb()
        {

           var delayCalculationsDict =  _timeStorage.ReturnConcurrentDictLock(_timeStorage._ActualDelayCalculations);

            foreach (var item in delayCalculationsDict)
            {
                var gettingKeys = delayCalculationsDict.Keys.FirstOrDefault(keyEntry => keyEntry.ClientId == item.Key.ClientId);

                 if (gettingKeys != null && delayCalculationsDict.ContainsKey(gettingKeys))
                 {
                     Console.WriteLine($"Existing client ID needs to be removed from the delay calculations dict, removing now -> {item.Key.ClientId}");

                     _timeStorage.RemoveFromConcurrentDict(_timeStorage._ActualDelayCalculations, gettingKeys);
                 } 

                 if(item.Value.TypeOfData == "BatchUnary")
                 {
                    if(!string.IsNullOrEmpty(item.Value.BatchRequestId))
                    {
                        DelayCalc transportingToDb = new DelayCalc
                        {
                            messageId = Guid.Parse(item.Value.BatchRequestId),
                            RequestType = item.Value.TypeOfData,
                            ClientUnique = Guid.Parse(item.Key.ClientId),
                            CommunicationType = "Batch",
                            DataIterations = item.Value.LengthOfData,
                            DataContent = item.Value.DataContents,
                            Delay = item.Value.Delay,
                            ClientInstance = null,
                        };

                        Console.WriteLine($"This is what the message and client id are before adding to the database, client ID : {transportingToDb.ClientUnique} : message ID: {transportingToDb.messageId}");

                        await _delayCalcRepo.AddToDbAsync(transportingToDb);

                        await _delayCalcRepo.SaveAsync();
                    }

                 }
                 else if(item.Value.TypeOfData == "Unary")
                 {
                    DelayCalc transportingToDb = new DelayCalc
                    {
                        messageId = Guid.Parse(item.Key.MessageId),
                        RequestType = item.Value.TypeOfData,
                        ClientUnique = Guid.Parse(item.Key.ClientId),
                        CommunicationType = item.Value.TypeOfData,
                        DataIterations = item.Value.LengthOfData,
                        DataContent = item.Value.DataContents,
                        Delay = item.Value.Delay,
                        ClientInstance = null,
                    };

                   
                    Console.WriteLine($"This is what the message and client id are before adding to the database, client ID : {transportingToDb.ClientUnique} : message ID: {transportingToDb.messageId}");

                    await _delayCalcRepo.AddToDbAsync(transportingToDb);

                    await _delayCalcRepo.SaveAsync();
                 }

              

                
            }
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

        private UnaryInfo MapToRequest(DateTime? timeOfRequest, string? typeOfData, int? lengthOfData, string? dataContent, string requestType, string? batchRequestId, TimeSpan? delay = null )
        {
           return _objectCreation.MappingToUnaryInfo(timeOfRequest, delay, typeOfData, lengthOfData, dataContent, requestType, batchRequestId );
        }

        private UnaryInfo MapToResponse(DateTime? timeOfRequest, string? typeOfData, int? lengthOfData, string? dataContent, string requestType, string? batchRequestId, TimeSpan? delay = null)
        {
            return _objectCreation.MappingToUnaryInfo(timeOfRequest, delay, typeOfData, lengthOfData, dataContent, requestType, batchRequestId);
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
                Log.Information($"Client ID : {details.ClientUnique} with overarching ID { details.BatchRequestId} handles batch messages -> {details.RequestId}");

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
