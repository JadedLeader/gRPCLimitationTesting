using DbManagerWorkerService.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.Interfaces.Repos
{
    public interface ICommunicationDelayRepo
    {

        public Task AddToDb(CommunicationDelay delay);

        public Task EmptyTable();

    }
}
