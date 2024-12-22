using DbManagerWorkerService.Abstracts;
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.Repositories
{
    public class SessionRepo : RepositoryAbstract<Session>, ISessionRepo
    {

        private readonly IDataContexts _dataContext;

        public SessionRepo(IDataContexts dataContext) : base(dataContext as DbContext)
        {
            _dataContext = dataContext;
        }

        public override Task<Session> AddToDbAsync(Session entity)
        {
            return base.AddToDbAsync(entity);
        }

        public override Task<Session> RemoveFromDbAsync(Session entity)
        {
            return base.RemoveFromDbAsync(entity);
        }

        public override Task<Session> UpdateDbAsync(Session entity)
        {
            return base.UpdateDbAsync(entity);
        }

        public override Task<IEnumerable<Session>> GetDbContent()
        {
            return base.GetDbContent();
        }

        public override async Task<Session> GetRecordViaId(Guid? recordId)
        {
            throw new NotImplementedException();
        }

        public async Task<Account> GetSessionViaAccountId(Guid accountUnique)
        {
            Account? sessionWithAccount = await _dataContext.Account.Include(s => s.Session).FirstOrDefaultAsync(a => a.AccountUnique == accountUnique);

            if (sessionWithAccount == null)
            {
                Log.Error($"No Session with ID can be linked with an account ID : {accountUnique}");
            }

            return sessionWithAccount;
        }
    }
}
