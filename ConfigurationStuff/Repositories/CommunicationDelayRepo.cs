
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Repos;

using Microsoft.EntityFrameworkCore;

using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
namespace DbManagerWorkerService.Repos
{
    public class CommunicationDelayRepo : ICommunicationDelayRepo
    {
        private readonly IDataContexts _dataContext;
        public CommunicationDelayRepo(IDataContexts dataContext)
        {
            _dataContext = dataContext;
        }

       


    }
}
