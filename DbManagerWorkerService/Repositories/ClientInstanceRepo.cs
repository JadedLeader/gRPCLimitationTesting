using DbManagerWorkerService.Abstracts;
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.Repositories
{
    public class ClientInstanceRepo : RepositoryAbstract<ClientInstance>, IClientInstanceRepo
    {

        private readonly IDataContexts _dataContext;

        public ClientInstanceRepo(IDataContexts dataContext) : base(dataContext as DbContext)
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

    }
}
