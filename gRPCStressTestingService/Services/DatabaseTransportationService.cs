using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using SharedCommonalities.TimeStorage;
using Serilog;
using System.Collections.Concurrent;
using Grpc.Core.Interceptors;

namespace gRPCStressTestingService.Services
{
    public class DatabaseTransportationService
    {
        private readonly RequestResponseTimeStorage _timeStorage;

        private readonly IDelayCalcRepo _delayCalcRepo;
        public DatabaseTransportationService(RequestResponseTimeStorage timeStorage, IDelayCalcRepo delayCalcRepo)
        {
            _timeStorage = timeStorage;
            _delayCalcRepo = delayCalcRepo;
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

                    Log.Information($"Batch unary data detected");

                    if (!string.IsNullOrEmpty(item.Value.BatchRequestId))
                    {
                        DelayCalc transportingToDb = new DelayCalc
                        {
                            messageId = Guid.Parse(item.Value.BatchRequestId),
                            RequestType = item.Value.TypeOfData,
                            ClientUnique = Guid.Parse(item.Key.ClientId),
                            CommunicationType = "Batch",
                            DataIterations = Convert.ToInt32(item.Value.DataIterations),
                            DataContent = item.Value.DataContentSize,
                            Delay = item.Value.Delay,
                            ClientInstance = null,
                            RecordCreation = DateTime.Now,
                        };

                        Log.Information($"This is what the message and client id are before adding to the database, client ID : {transportingToDb.ClientUnique} : message ID: {transportingToDb.messageId}");

                        await _delayCalcRepo.AddToDbAsync(transportingToDb);

                        await _delayCalcRepo.SaveAsync();
                    }

                }
                else if (item.Value.TypeOfData == "Unary")
                {

                    Log.Information($"Unary data detected");

                    DelayCalc transportingToDb = new DelayCalc
                    {
                        messageId = Guid.Parse(item.Key.MessageId),
                        RequestType = item.Value.TypeOfData,
                        ClientUnique = Guid.Parse(item.Key.ClientId),
                        CommunicationType = item.Value.TypeOfData,
                        DataIterations = Convert.ToInt32(item.Value.DataIterations),
                        DataContent = item.Value.DataContentSize,
                        Delay = item.Value.Delay,
                        ClientInstance = null,
                        RecordCreation = DateTime.Now,
                    };


                    Log.Information($"This is what the message and client id are before adding to the database, client ID : {transportingToDb.ClientUnique} : message ID: {transportingToDb.messageId}");

                    await _delayCalcRepo.AddToDbAsync(transportingToDb);

                    await _delayCalcRepo.SaveAsync();
                }
                else if (item.Value.TypeOfData == "Streaming")
                {

                    Log.Information($"Streaming data detected");

                    DelayCalc transportingToDb = new DelayCalc
                    {
                        messageId = Guid.Parse(item.Key.MessageId),
                        RequestType = item.Value.TypeOfData,
                        ClientUnique = Guid.Parse(item.Key.ClientId),
                        CommunicationType = item.Value.TypeOfData,
                        DataIterations = Convert.ToInt32(item.Value.DataIterations),
                        DataContent = item.Value.DataContentSize,
                        Delay = item.Value.Delay,
                        ClientInstance = null,
                        RecordCreation = DateTime.Now,
                        
                    };


                    Log.Information($"This is what the message and client id are before adding to the database, client ID : {transportingToDb.ClientUnique} : message ID: {transportingToDb.messageId}");

                    await _delayCalcRepo.AddToDbAsync(transportingToDb);

                    await _delayCalcRepo.SaveAsync();

                }
                else if(item.Value.TypeOfData == "StreamingBatch")
                {
                    Log.Information($"streaming batch detected");

                    DelayCalc newDelay = new DelayCalc
                    {
                        messageId = Guid.Parse(item.Key.MessageId),
                        RequestType = item.Value.TypeOfData,
                        ClientUnique = Guid.Parse(item.Key.ClientId),
                        CommunicationType = item.Value.TypeOfData,
                        DataIterations = Convert.ToInt32(item.Value.DataIterations),
                        DataContent = item.Value.DataContentSize,
                        Delay = item.Value.Delay,
                        ClientInstance = null,
                        RecordCreation = DateTime.Now,
                    };

                    await _delayCalcRepo.AddToDbAsync(newDelay);

                    await _delayCalcRepo.SaveAsync();

                
                }

            }
        }

    }
}
