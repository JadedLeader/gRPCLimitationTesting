
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

        /*private void PopulatingNewDict()
        {
            _DelayCalculations = RequestResponseTimeStorage.ReturnDelayCalculations();
        } */

        private Dictionary<string, UnaryInfo> ReturningDict()
        {
            var delayCalculationsDict = _requestResponseTimeStorage.ReturnDictionary(_requestResponseTimeStorage._ActualDelayCalculations);

            return delayCalculationsDict;
        }

        public async Task EmptyTable()
        {
           await _communicationDelayRepo.EmptyTable();
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

                //this needs to be edited for streaming requests as right now for a batch we dont care about each item in the list as they all have the same timestamp 
                //however with a streaming request, we're going to care about every timing within that list

                foreach (var item in delayCalculationsDict)
                {

                    if(delayCalculationsDict.ContainsKey(item.Key))
                    {
                        Console.WriteLine($"item keys guid value to be removed -> {item.Key}");

                        Console.WriteLine($"item already exists in the dictionary, removing it now -> {item.Key}");

                        _requestResponseTimeStorage.RemoveFromDictionary(_requestResponseTimeStorage._ActualDelayCalculations, item.Key);

                       // RemoveFromDict(item.Key);
                    }

                    
                    var transportingToDb = new CommunicationDelay()
                    {
                        DelayGuid = item.Key,
                        CommunicationType = item.Value.TypeOfData,
                        DataLength = item.Value.LengthOfData.Value,
                        Delay = item.Value.Delay.Value,
                        RequestType = item.Value.TypeOfData
                    };

                    if (item.Value.TypeOfData.Contains("Batch"))
                    {
                        transportingToDb.CommunicationType = "Batch";
                    }

                    await _communicationDelayRepo.AddToDb(transportingToDb);

                    
                  
                    Console.WriteLine($"AMOUNT OF THINGS IN THE CLIENT STORAGE -> { _storage.ReturnDictionary(_storage.Clients).Count}");
                    Console.WriteLine($"THIS IS THE MEMORY ADRESS  FOR THE SINGLETON IN THE DELAY SERVICE -> {_storage.GetHashCode()}");



                }
            } 
            catch(Exception ex)
            {
                Console.WriteLine($"something happened in the delay service -> {ex.Message}");
            }
        }

    }
}
