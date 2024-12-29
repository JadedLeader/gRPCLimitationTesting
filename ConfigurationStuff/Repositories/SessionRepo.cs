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

namespace DbManagerWorkerService.Repositories
{
    public class SessionRepo : RepositoryAbstract<Session>, ISessionRepo
    {

        private readonly IDataContexts _dataContext;

        public SessionRepo(IDataContexts dataContext) : base(dataContext as DbContext)
        {
            _dataContext = dataContext;

            Console.WriteLine($"SessionRepo is using DbContext instance: {_dataContext.GetHashCode()}");
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

        public async Task<List<Session>> GetClientInstancesBasedOffSessions(Guid sessionUnique)
        {
            List<Session> getClientInstances = await _dataContext.Session.Include(ci => ci.ClientInstance).Where(s => s.SessionUnique == sessionUnique).ToListAsync();

            if (getClientInstances == null)
            {
                Log.Error($"Could not retrieve Client instances based off the session ID {sessionUnique}");
            }

            return getClientInstances;
        }

       

        public async Task<Session> getSessionWithChat(Guid sessionUnique)
        {
            Session? session = await _dataContext.Session
                .Include(s => s.ClientInstance)
                .FirstOrDefaultAsync(s => s.SessionUnique == sessionUnique);

            if (session == null)
            {
                throw new Exception($"Session {sessionUnique} not found.");
            }

            // Log the rowversion as hex to confirm EF loaded it
           

           // await SaveAsync();

            return session;
        }

        public override Task SaveAsync()
        {
            return base.SaveAsync();
        }

        public override Task ReloadAsync(Session entity)
        {
            return base.ReloadAsync(entity);
        }

        public async Task<Session?> GetSessionWithClientInstances(Guid sessionUnique)
        {
            return await _dataContext.Session
                .Include(s => s.ClientInstance)
                .FirstOrDefaultAsync(s => s.SessionUnique == sessionUnique);
        }


    }
}
