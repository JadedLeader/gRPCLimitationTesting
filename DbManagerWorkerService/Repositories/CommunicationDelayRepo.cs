using DbManagerWorkerService.DbContexts;
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Repos;
using DbManagerWorkerService.DbModels;
using Microsoft.EntityFrameworkCore;
using DbManagerWorkerService.Interfaces.Repos;
namespace DbManagerWorkerService.Repos
{
    public class CommunicationDelayRepo : ICommunicationDelayRepo
    {
        private readonly IDataContexts _dataContext;
        public CommunicationDelayRepo(IDataContexts dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task AddToDb(CommunicationDelay delay)
        {
            try
            {
                _dataContext.CommunicationDelay.Add(delay);

                await _dataContext.SaveChangesAsync();
            } 
            catch(DbUpdateException ex)
            {
                Console.WriteLine($"this is the inner exception -> {ex.InnerException.Message}");
            }
        }

        public async Task EmptyTable()
        {
            foreach(var delay in _dataContext.CommunicationDelay)
            {
                _dataContext.CommunicationDelay.Remove(delay);
            }

            await _dataContext.SaveChangesAsync();


            Console.WriteLine($"table has been emptied");
        }



    }
}
