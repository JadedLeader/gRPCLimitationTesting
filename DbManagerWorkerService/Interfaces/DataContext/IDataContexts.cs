using DbManagerWorkerService.DbModels; 
using Microsoft.EntityFrameworkCore;

namespace DbManagerWorkerService.Interfaces.DataContext
{
    public interface IDataContexts
    {

        public DbSet<CommunicationDelay> CommunicationDelay { get; set; }

        public Task<int> SaveChangesAsync();

    }
}
