using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using SharedCommonalities.TimeStorage;
using Serilog;
using System.Collections.Concurrent;
using Grpc.Core.Interceptors;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.Services
{
    public class DatabaseTransportationService
    {
        private readonly RequestResponseTimeStorage _timeStorage;

        private readonly IDelayCalcRepo _delayCalcRepo;

        private readonly IClientInstanceRepo _clientInstanceRepo;
        public DatabaseTransportationService(RequestResponseTimeStorage timeStorage, IDelayCalcRepo delayCalcRepo, IClientInstanceRepo clientInstanceRepo)
        {
            _timeStorage = timeStorage;
            _delayCalcRepo = delayCalcRepo;
            _clientInstanceRepo = clientInstanceRepo;
        }

        public async Task AddingDelayToDb()
        {

            ConcurrentDictionary<ClientMessageId, UnaryInfo> delayCalculationsDict = _timeStorage.ReturnConcurrentDictLock(_timeStorage._ActualDelayCalculations);

            foreach (var item in delayCalculationsDict)
            {
                ClientMessageId gettingKeys = delayCalculationsDict.Keys.FirstOrDefault(keyEntry => keyEntry.ClientId == item.Key.ClientId);

                if (gettingKeys != null && delayCalculationsDict.ContainsKey(gettingKeys))
                {
                   Log.Information($"Existing client ID needs to be removed from the delay calculations dict, removing now -> {item.Key.ClientId}");

                    _timeStorage.RemoveFromConcurrentDict(_timeStorage._ActualDelayCalculations, gettingKeys);
                }

                if (item.Value.TypeOfData == "BatchUnary")
                {
                    await HandleBatchUnaryCase(item);

                }
                else if (item.Value.TypeOfData == "Unary")
                {

                    await HandleUnaryCase(item);
                }
                else if (item.Value.TypeOfData == "Streaming")
                {
                   
                    await HandleStreamingCase(item);

                }
                else if(item.Value.TypeOfData == "StreamingBatch")
                {
                  
                    await HandleStreamingBatchCase(item);
                
                }

            }
        }

        private async Task HandleUnaryCase(KeyValuePair<ClientMessageId, UnaryInfo> clientWithInfo)
        {
            Log.Information($"Unary data detected");

            DelayCalc transportingToDb = await CreateDelayCalc(Guid.Parse(clientWithInfo.Key.MessageId), clientWithInfo.Value.TypeOfData, Guid.Parse(clientWithInfo.Key.ClientId), clientWithInfo.Value.TypeOfData,
                Convert.ToInt32(clientWithInfo.Value.DataIterations), clientWithInfo.Value.DataContentSize, clientWithInfo.Value.Delay, null, DateTime.Now);

            Log.Information($"This is what the message and client id are before adding to the database, client ID : {transportingToDb.ClientUnique} : message ID: {transportingToDb.messageId}");

            var gettingClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(transportingToDb.ClientUnique);

            transportingToDb.ClientInstance = gettingClientInstance;

            await AddToDbAndSave(transportingToDb);
        }

        private async Task HandleBatchUnaryCase(KeyValuePair<ClientMessageId, UnaryInfo> clientWithInfo)
        {
            Log.Information($"Batch unary data detected");

            if (!string.IsNullOrEmpty(clientWithInfo.Value.BatchRequestId))
            {

                DelayCalc transportingToDb = await CreateDelayCalc(Guid.Parse(clientWithInfo.Value.BatchRequestId), clientWithInfo.Value.TypeOfData, Guid.Parse(clientWithInfo.Key.ClientId), "Batch",
                    Convert.ToInt32(clientWithInfo.Value.DataIterations), clientWithInfo.Value.DataContentSize, clientWithInfo.Value.Delay, null, DateTime.Now);

                Log.Information($"This is what the message and client id are before adding to the database, client ID : {transportingToDb.ClientUnique} : message ID: {transportingToDb.messageId}");

                var gettingClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(transportingToDb.ClientUnique);

                transportingToDb.ClientInstance = gettingClientInstance;

                await AddToDbAndSave(transportingToDb);
            }
        }

        private async Task HandleStreamingCase(KeyValuePair<ClientMessageId, UnaryInfo> clientWithInfo)
        {
            Log.Information($"Streaming data detected");

            DelayCalc transportingToDb = await CreateDelayCalc(Guid.Parse(clientWithInfo.Key.MessageId), clientWithInfo.Value.TypeOfData, Guid.Parse(clientWithInfo.Key.ClientId), clientWithInfo.Value.TypeOfData,
                Convert.ToInt32(clientWithInfo.Value.DataIterations), clientWithInfo.Value.DataContentSize, clientWithInfo.Value.Delay, null, DateTime.Now);

            Log.Information($"This is what the message and client id are before adding to the database, client ID : {transportingToDb.ClientUnique} : message ID: {transportingToDb.messageId}");

            var gettingClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(transportingToDb.ClientUnique);

            transportingToDb.ClientInstance = gettingClientInstance;

            await AddToDbAndSave(transportingToDb);
        }

        private async Task HandleStreamingBatchCase(KeyValuePair<ClientMessageId, UnaryInfo> clientWithInfo)
        {
            Log.Information($"streaming batch detected");

            DelayCalc transportingToDb = await CreateDelayCalc(Guid.Parse(clientWithInfo.Key.MessageId), clientWithInfo.Value.TypeOfData, Guid.Parse(clientWithInfo.Key.ClientId), clientWithInfo.Value.TypeOfData,
                Convert.ToInt32(clientWithInfo.Value.DataIterations), clientWithInfo.Value.DataContentSize, clientWithInfo.Value.Delay, null, DateTime.Now);

            var gettingClientInstance = await _clientInstanceRepo.GetClientInstanceViaClientUnique(transportingToDb.ClientUnique);

            transportingToDb.ClientInstance = gettingClientInstance;

            await AddToDbAndSave(transportingToDb);
        }

        private async Task<ClientInstance> IdentifyingClientInstance(Guid? clientUnique, DelayCalc delay)
        {
            ClientInstance client = await _clientInstanceRepo.GetClientInstanceViaClientUnique(clientUnique.Value);

            if(client == null)
            {
                Log.Warning($"No client instance with the client unique : {clientUnique} could be found");
            }

            if(client.DelayCalcs == null)
            {
                client.DelayCalcs = new List<DelayCalc>();

                client.DelayCalcs.Add(delay);
            }

            return client;
        }

        private async Task<DelayCalc> CreateDelayCalc(Guid messageId,  string? requestType, Guid? clientUnique, string? communicationType, int dataIterations, string? dataContent, TimeSpan? delay, 
             ClientInstance? clientInstance, DateTime recordCreation)
        {
            DelayCalc delayCalc = new DelayCalc
            {
                messageId = messageId,
                RequestType = requestType,
                CommunicationType = communicationType,
                DataIterations = dataIterations,
                DataContent = dataContent,
                Delay = delay,
                ClientInstance = clientInstance,
                RecordCreation = recordCreation,

            };

            return delayCalc;
        }

        private async Task AddToDbAndSave(DelayCalc delayInstance)
        {
            await _delayCalcRepo.AddToDbAsync(delayInstance);

            await _delayCalcRepo.SaveAsync();
        }

    }
}
