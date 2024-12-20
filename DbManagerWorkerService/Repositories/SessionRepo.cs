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
    public class SessionRepo : RepositoryAbstract<SessionRepo>, ISessionRepo
    {

        private readonly IDataContexts _dataContext;

        public SessionRepo(IDataContexts dataContext) : base(dataContext as DbContext)
        {
            _dataContext = dataContext;
        }

        public override Task<SessionRepo> AddToDbAsync(SessionRepo entity)
        {
            return base.AddToDbAsync(entity);
        }

        public override Task<SessionRepo> RemoveFromDbAsync(SessionRepo entity)
        {
            return base.RemoveFromDbAsync(entity);
        }

        public override Task<SessionRepo> UpdateDbAsync(SessionRepo entity)
        {
            return base.UpdateDbAsync(entity);
        }

        public override Task<IEnumerable<SessionRepo>> GetDbContent()
        {
            return base.GetDbContent();
        }

        public override async Task<SessionRepo> GetRecordViaId(Guid? recordId)
        {
            throw new NotImplementedException();
        }
    }
}
