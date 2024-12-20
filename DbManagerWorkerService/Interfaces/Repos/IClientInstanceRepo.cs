using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.Interfaces.Repos
{
    public interface IClientInstanceRepo
    {

        public Task<ClientInstance> AddToDbAsync(ClientInstance entity);

        public Task<ClientInstance> RemoveFromDbAsync(ClientInstance entity);


        public Task<IEnumerable<ClientInstance>> GetDbContent();
        

    }
}
