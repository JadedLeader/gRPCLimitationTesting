
using DbManagerWorkerService.DbContexts;
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Repos;
using DbManagerWorkerService.DbModels;
using SharedCommonalities.Interfaces.TimeStorage;
using DbManagerWorkerService.Services;
using DbManagerWorkerService.Interfaces;

namespace DbManagerWorkerService.Services
{
    public class CommunicationDelayService : ICommunicationDelayService
    {

        private readonly IRequestResponseTimeStorage _requestResponseTimeStorage;

        private readonly ICommunicationDelayRepo _communicationDelayRepo;
        public CommunicationDelayService(IRequestResponseTimeStorage requestResponseTimeStorage, ICommunicationDelayRepo communicationDelayRepo)
        {
            _requestResponseTimeStorage = requestResponseTimeStorage;
            _communicationDelayRepo = communicationDelayRepo;
        }

        public async Task AddingDelayCalculationsToDb()
        {
            try
            {

                var delayCalculationsDict = _requestResponseTimeStorage.ReturnDelayCalculations();

                if (delayCalculationsDict.Count == 0)
                {
                    Console.WriteLine($"nothing to add to db");
                }

                foreach (var item in delayCalculationsDict)
                {
                    var transportingToDb = new CommunicationDelay()
                    {
                        DelayGuid = item.Key,
                        CommunicationType = item.Value.TypeOfData,
                        DataLength = item.Value.LengthOfData.Value,
                        Delay = item.Value.Delay.Value,
                    };

                    await _communicationDelayRepo.AddToDb(transportingToDb);

                }
            } 
            catch(Exception ex)
            {
                Console.WriteLine($"something happened in the delay service -> {ex.Message}");
            }
        }

    }
}
