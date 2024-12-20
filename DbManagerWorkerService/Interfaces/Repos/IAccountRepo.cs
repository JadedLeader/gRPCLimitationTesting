using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.Interfaces.Repos
{
    public interface IAccountRepo
    {

        public Task<Account> AddToDbAsync(Account entity);

        public Task<Account> RemoveFromDbAsync(Account entity);

        public Task<Account> UpdateDbAsync(Account entity);

        public Task<IEnumerable<Account>> GetDbContent();

        public Task<Account> GetAccountViaId(Guid recordId);

        public Task<Account> GetAccountViaUsername(string username);

        public Task<Account> UpdateAuthUnique(Account entity, Guid tokenUnique, AuthToken tokenInstance);

        public Task<Account> LinkAccountWithTokenViaId(Guid accountId);



    }
}
