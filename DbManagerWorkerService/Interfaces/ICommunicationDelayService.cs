namespace DbManagerWorkerService.Interfaces
{
    public interface ICommunicationDelayService
    {

        public Task AddingDelayCalculationsToDb();

        public Task EmptyTable();

    }
}
