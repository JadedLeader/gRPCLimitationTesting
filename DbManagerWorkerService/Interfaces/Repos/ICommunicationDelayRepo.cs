using DbManagerWorkerService.DbModels;

namespace DbManagerWorkerService.Repos
{
    public interface ICommunicationDelayRepo
    {

        public Task AddToDb(CommunicationDelay delay);

    }
}
