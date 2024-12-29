using DbManagerWorkerService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationStuff.DbModels;

namespace ConfigurationStuff.Interfaces.Repos
{
    public interface ISessionRepo
    {

        public Task<Session> AddToDbAsync(Session entity);



        public Task<Session> RemoveFromDbAsync(Session entity);



        public Task<Session> UpdateDbAsync(Session entity);



        public Task<IEnumerable<Session>> GetDbContent();



        public Task<Session> GetRecordViaId(Guid? recordId);
        

        public Task<Account> GetSessionViaAccountId(Guid accountUnique);

        public Task<List<Session>> GetClientInstancesBasedOffSessions(Guid sessionUnique);

        public Task<Session?> GetSessionWithClientInstances(Guid sessionUnique);

        public Task<Session> getSessionWithChat(Guid sessionUnique);

        public Task SaveAsync();

        public Task ReloadAsync(Session entity);
       


    }
}
