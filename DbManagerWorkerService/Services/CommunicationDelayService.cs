
using DbManagerWorkerService.DbContexts;
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Repos;
using DbManagerWorkerService.DbModels;
using SharedCommonalities.Interfaces.TimeStorage;
using DbManagerWorkerService.Services;
using DbManagerWorkerService.Interfaces;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.ReturnModels.ReturnTypes;
using SharedCommonalities.Storage;
using DbManagerWorkerService.Interfaces.Repos;

namespace DbManagerWorkerService.Services
{
    public class CommunicationDelayService : ICommunicationDelayService
    {

        private readonly ClientStorage _storage;

        public Dictionary<string, UnaryInfo> _DelayCalculations = new Dictionary<string, UnaryInfo>();

        private readonly RequestResponseTimeStorage _requestResponseTimeStorage;

        private readonly ICommunicationDelayRepo _communicationDelayRepo;
        public CommunicationDelayService( ICommunicationDelayRepo communicationDelayRepo, ClientStorage storage, RequestResponseTimeStorage requestResponseTimeStorage)
        {
            _communicationDelayRepo = communicationDelayRepo;
            _storage = storage;
            _requestResponseTimeStorage = requestResponseTimeStorage;
        }

        private void RemoveFromDict(string guid)
        {
            _DelayCalculations.Remove(guid);
        }

        private Dictionary<ClientMessageId, UnaryInfo> ReturningDict()
        {
            var delayCalculationsDict = _requestResponseTimeStorage.ReturnDictionary(_requestResponseTimeStorage._ActualDelayCalculations);

            return delayCalculationsDict;
        }

       /* public async Task EmptyTable()
        {
           await _communicationDelayRepo.EmptyTable();
        } */

       /* public async Task AddingDelayCalculationsToDb()
        {
            try
            {
                var delayCalculationsDict = ReturningDict();

                if (delayCalculationsDict.Count == 0)
                {
                    Console.WriteLine($"nothing to add to db");
                }

                //this needs to be edited for streaming requests as right now for a batch we dont care about each item in the list as they all have the same timestamp 
                //however with a streaming request, we're going to care about every timing within that list

                foreach (var item in delayCalculationsDict)
                {

                    var gettingKeys = delayCalculationsDict.Keys.FirstOrDefault(keyEntry => keyEntry.ClientId == item.Key.ClientId);

                    if(gettingKeys != null && delayCalculationsDict.ContainsKey(gettingKeys))
                    {
                        Console.WriteLine($"Existing client ID needs to be removed from the delay calculations dict, removing now -> {item.Key.ClientId}");

                        _requestResponseTimeStorage.RemoveFromDictionary(_requestResponseTimeStorage._ActualDelayCalculations, gettingKeys);
                    } 

                    var transportingToDb = new CommunicationDelay()
                    {
                        ClientId = item.Key.ClientId,
                        MessageDelayId = item.Key.MessageId,
                        CommunicationType = item.Value.TypeOfData,
                        DataIterations = item.Value.LengthOfData.Value,
                        Delay = item.Value.Delay.Value,
                        RequestType = item.Value.TypeOfData,
                        DataContents = item.Value.DataContents,
                    };

                    if (item.Value.TypeOfData.Contains("Batch"))
                    {
                        transportingToDb.CommunicationType = "Batch";
                    }

                   // await _communicationDelayRepo.AddToDb(transportingToDb);

                    var delaydick = ReturningDict();
                }
            } 
            catch(Exception ex)
            {
                Console.WriteLine($"something happened in the delay service -> {ex.Message}");
            } */
        

    }
}
