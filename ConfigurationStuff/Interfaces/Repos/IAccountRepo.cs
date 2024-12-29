using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationStuff.DbModels;

namespace ConfigurationStuff.Interfaces.Repos
{
    public interface IAccountRepo
    {

        public Task<Account> AddToDbAsync(Account entity);

        public Task<Account> RemoveFromDbAsync(Account entity);

        public Task<Account> UpdateDbAsync(Account entity);

        public Task<IEnumerable<Account>> GetDbContent();

        public Task<Account> GetAccountViaId(Guid recordId);

        public Task<Account> GetAccountViaUsername(string username);

        public Task<Account> UpdateAuthUnique(Account entity, Guid? tokenUnique, AuthToken tokenInstance);

        public Task<Account> LinkAccountWithTokenViaId(Guid accountId);

        public Task<Account> GetAccountViaToken(Guid tokenUnique);

        public Task<Account> LinkAccountWithSessionViaId(Guid accountId);

        public Task<Account> LinkAccountWithSessionViaUsername(string username);

        public Task<Account> GetSessionAndAccountViaSessionId(Guid sessionUnique);

        public Task<Account> GetAccountWithSessionClientInstance(string username);
        public Task SaveAsync();



    }
}
