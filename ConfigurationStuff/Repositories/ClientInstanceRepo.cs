using ConfigurationStuff.Abstracts;
using DbManagerWorkerService.Interfaces.DataContext;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using ConfigurationStuff.DbContexts;

namespace DbManagerWorkerService.Repositories
{
    public class ClientInstanceRepo : RepositoryAbstract<ClientInstance>, IClientInstanceRepo
    {

        private readonly IDataContexts _dataContext;

        public ClientInstanceRepo(IDataContexts dataContext) : base(dataContext as DataContexts)
        {
            _dataContext = dataContext;
        }

        public override Task<ClientInstance> AddToDbAsync(ClientInstance entity)
        {
            return base.AddToDbAsync(entity);
        }

        public override Task<ClientInstance> RemoveFromDbAsync(ClientInstance entity)
        {
            return base.RemoveFromDbAsync(entity);
        }

        public override Task<ClientInstance> UpdateDbAsync(ClientInstance entity)
        {
            return base.UpdateDbAsync(entity);
        }

        public override Task<IEnumerable<ClientInstance>> GetDbContent()
        {
            return base.GetDbContent();
        }

        public override async Task<ClientInstance> GetRecordViaId(Guid? recordId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ClientInstance>> GetClientInstancesViaSessionId(Guid sessionUnique)
        {
            List<ClientInstance> clientInstances = _dataContext.ClientInstance.Where(su => su.SessionUnique == sessionUnique).ToList();

            if(clientInstances == null)
            {
                Log.Error($"No client instances exist with the session ID: {sessionUnique}");
            }

            return clientInstances;
        }

        public async Task<List<ClientInstance>> AddBatchToDbAsync(List<ClientInstance> clientInstances)
        {
            _dataContext.ClientInstance.AddRange(clientInstances);


            return clientInstances.ToList();
        }

        public async Task<List<ClientInstance>> UpdateRangeToDbAsync(ICollection<ClientInstance> clientInstances)
        {
            _dataContext.ClientInstance.UpdateRange(clientInstances);
            return clientInstances.ToList();
        }

        public async Task<List<ClientInstance>> GetCLientsFromSessionId(Guid sessionUnique)
        {
            List<ClientInstance> clientsFromSession =  await _dataContext.ClientInstance.Where(st => st.SessionUnique == sessionUnique).ToListAsync();

            return clientsFromSession;
        }

        public async Task<List<ClientInstance>> RemoveRange(List<ClientInstance> clientToRemove)
        {
            _dataContext.ClientInstance.RemoveRange(clientToRemove);

            return clientToRemove.ToList();
        }
        public override Task RemoveRangeAsync(List<ClientInstance> clientList)
        {
            return base.RemoveRangeAsync(clientList);
        }

        public override Task SaveAsync()
        {
            return base.SaveAsync();
        }
    }
}
