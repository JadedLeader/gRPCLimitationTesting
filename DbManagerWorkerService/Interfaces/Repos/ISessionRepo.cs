using DbManagerWorkerService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.Interfaces.Repos
{
    public interface ISessionRepo
    {

        public Task<Session> AddToDbAsync(Session entity);



        public Task<Session> RemoveFromDbAsync(Session entity);



        public Task<Session> UpdateDbAsync(Session entity);



        public Task<IEnumerable<Session>> GetDbContent();



        public Task<Session> GetRecordViaId(Guid? recordId);
        

        public Task<Account> GetSessionViaAccountId(Guid accountUnique);


    }
}
