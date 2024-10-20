using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementWorkerService.Interfaces
{
    public interface IClientManagementService
    {

        public Task RemovingSentRequestsFromClientPool();

        public Task thing();

    }
}
