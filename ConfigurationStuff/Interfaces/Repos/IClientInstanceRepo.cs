using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationStuff.DbModels;

namespace ConfigurationStuff.Interfaces.Repos
{
    public interface IClientInstanceRepo
    {

        public Task<ClientInstance> AddToDbAsync(ClientInstance entity);

        public Task<ClientInstance> RemoveFromDbAsync(ClientInstance entity);

        public Task<ClientInstance> UpdateDbAsync(ClientInstance entity);
        public Task<IEnumerable<ClientInstance>> GetDbContent();

        public Task<List<ClientInstance>> GetClientInstancesViaSessionId(Guid sessionUnique);

        public Task<List<ClientInstance>> AddBatchToDbAsync(List<ClientInstance> clientInstances);

        public Task<List<ClientInstance>> UpdateRangeToDbAsync(ICollection<ClientInstance> clientInstances);

        public Task<List<ClientInstance>> GetCLientsFromSessionId(Guid sessionUnique);

        public Task<List<ClientInstance>> RemoveRange(List<ClientInstance> clientToRemove);

        public Task RemoveRangeAsync(List<ClientInstance> clientList);

        public Task SaveAsync();



    }
}
