using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Repos;
using DbManagerWorkerService.Services;
using DbManagerWorkerService.Interfaces;
using SharedCommonalities.Storage;
using SharedCommonalities.ReturnModels.ReturnTypes;
using SharedCommonalities.TimeStorage;
using ConfigurationStuff.Interfaces.Repos;
using ConfigurationStuff.DbModels;
using Serilog;

namespace DbManagerWorkerService.Services
{
    public class CommunicationDelayService : ICommunicationDelayService
    {

        private readonly ClientStorage _storage;

        private readonly RequestResponseTimeStorage _requestResponseTimeStorage;

        private readonly IDelayCalcRepo _delayCalcRepo;

        public CommunicationDelayService(IDelayCalcRepo delayCalcRepo, ClientStorage storage, RequestResponseTimeStorage requestResponseTimeStorage)
        {
            _delayCalcRepo = delayCalcRepo;
            _storage = storage;
            _requestResponseTimeStorage = requestResponseTimeStorage;
        }

        private Dictionary<ClientMessageId, UnaryInfo> ReturningDict()
        {
            var delayCalculationsDict = _requestResponseTimeStorage.ReturnDictionary(_requestResponseTimeStorage._ActualDelayCalculations);

            return delayCalculationsDict;
        } 

        public async Task AddingDelayCalculationsToDb()
        {
            try
            {
                var delayCalculationsDict = ReturningDict();



                if (delayCalculationsDict.Count == 0)
                {
                    Console.WriteLine($"nothing to add to db");
                }
                else
                {
                    Console.WriteLine("SOMETHING TO ADD TO DB");
                }
                    

                //this needs to be edited for streaming requests as right now for a batch we dont care about each item in the list as they all have the same timestamp 
                //however with a streaming request, we're going to care about every timing within that list

                foreach (var item in delayCalculationsDict)
                {
                    var gettingKeys = delayCalculationsDict.Keys.FirstOrDefault(keyEntry => keyEntry.ClientId == item.Key.ClientId);

                    if (gettingKeys != null && delayCalculationsDict.ContainsKey(gettingKeys))
                    {
                        Console.WriteLine($"Existing client ID needs to be removed from the delay calculations dict, removing now -> {item.Key.ClientId}");

                        _requestResponseTimeStorage.RemoveFromDictionary(_requestResponseTimeStorage._ActualDelayCalculations, gettingKeys);
                    }

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
                    
                    if (item.Value.TypeOfData.Contains("Batch"))
                    {
                        transportingToDb.CommunicationType = "Batch";
                    }

                    await _delayCalcRepo.AddToDbAsync(transportingToDb);

                    await _delayCalcRepo.SaveAsync();

                    var delaydick = ReturningDict();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"something happened in the delay service -> {ex.Message}");
            }
        }
        

    }
}
