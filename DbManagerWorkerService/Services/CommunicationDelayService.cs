
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

namespace DbManagerWorkerService.Services
{
    public class CommunicationDelayService : ICommunicationDelayService
    {

        public Dictionary<string, UnaryInfo> _DelayCalculations = new Dictionary<string, UnaryInfo>();

        private readonly ICommunicationDelayRepo _communicationDelayRepo;
        public CommunicationDelayService( ICommunicationDelayRepo communicationDelayRepo)
        {
            _communicationDelayRepo = communicationDelayRepo;
        }

        private void RemoveFromDict(string guid)
        {
            _DelayCalculations.Remove(guid);
        }

        private void PopulatingNewDict()
        {
            _DelayCalculations = RequestResponseTimeStorage.ReturnDelayCalculations();
        }

        private Dictionary<string, UnaryInfo> ReturningDict()
        {
            return _DelayCalculations;
        }

        public async Task EmptyTable()
        {
           await _communicationDelayRepo.EmptyTable();
        }

        public async Task AddingDelayCalculationsToDb()
        {
            try
            {

                PopulatingNewDict();
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
                        RemoveFromDict(item.Key);
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

                  
                    Console.WriteLine($"AMOUNT OF THINGS IN THE CLIENT STORAGE -> {ClientStorage.ReturnDictionary().Count}");



                }
            } 
            catch(Exception ex)
            {
                Console.WriteLine($"something happened in the delay service -> {ex.Message}");
            }
        }

    }
}
