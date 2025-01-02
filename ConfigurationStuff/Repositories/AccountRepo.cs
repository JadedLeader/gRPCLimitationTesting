using DbManagerWorkerService.Interfaces.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using ConfigurationStuff.Abstracts;
using ConfigurationStuff.DbContexts;

namespace ConfigurationStuff.Repositories
{
    public class AccountRepo : RepositoryAbstract<Account>, IAccountRepo
    {


        private readonly IDataContexts _dataContext;

        public AccountRepo(IDataContexts dataContext) : base(dataContext as DataContexts)
        {
            _dataContext = dataContext;
        }

        public override Task<Account> AddToDbAsync(Account entity)
        {
            return base.AddToDbAsync(entity);
        }

        public override Task<Account> RemoveFromDbAsync(Account entity)
        {
            return base.RemoveFromDbAsync(entity);
        }

        public override Task<Account> UpdateDbAsync(Account entity)
        {
            return base.UpdateDbAsync(entity);
        }

        public async Task<Account> UpdateAuthUnique(Account entity, Guid? tokenUnique, AuthToken tokenInstance)
        {
            entity.AuthUnique = tokenUnique;
            entity.AuthToken = tokenInstance;

            _dataContext.Account.Update(entity);


            return entity;
        }

        public override Task<IEnumerable<Account>> GetDbContent()
        {
            return base.GetDbContent();
        }

        public override Task<Account> GetRecordViaId(Guid? recordId)
        {
            throw new NotImplementedException();
        }

        public async Task<Account> GetAccountViaId(Guid recordId)
        {
            Account? accountViaId = await _dataContext.Account.FirstOrDefaultAsync(u => u.AccountUnique == recordId);

            if(accountViaId == null)
            {
                Log.Information($"An account with id {recordId} cannot be retrieved");

            }

            return accountViaId;

        }

        public async Task<Account> GetAccountViaUsername(string username)
        {
            Account? accountViaUsername = await _dataContext.Account
                .Include(s => s.Session)
                .FirstOrDefaultAsync(u => u.Username == username);

            if(accountViaUsername == null)
            {

                Console.WriteLine($"THIS DID NOT WORK THE ACCOUNT VIA USERNAME WAS NULL");
                Log.Information($"An account with username {username} cannot be retrieved");
            }

            return accountViaUsername;
        }

        public async Task<Account> LinkAccountWithTokenViaId(Guid accountId)
        {
            Account? accountWithToken = await _dataContext.Account.Include(t => t.AuthToken).FirstOrDefaultAsync(u => u.AccountUnique == accountId);

            if(accountWithToken == null)
            {
                Log.Error($"A token cannot be paired with account with ID : {accountId}");
            }

            return accountWithToken;
        }

        public async Task<Account> GetAccountViaToken(Guid tokenUnique)
        {
            Account? tokenWithAccount = await _dataContext.Account.Include(at => at.AuthToken).FirstOrDefaultAsync(t => t.AuthUnique == tokenUnique);

            if(tokenWithAccount == null)
            {
                Log.Error($"No token could be linked with the account via a token ID of {tokenUnique}.");
            }

            return tokenWithAccount;
        }

        public async Task<Account> LinkAccountWithSessionViaId(Guid accountId)
        {
            Account? accountWithToken = await _dataContext.Account.Include(t => t.Session).FirstOrDefaultAsync(u => u.AccountUnique == accountId);

            if (accountWithToken == null)
            {
                Log.Error($"A token cannot be paired with account with ID : {accountId}");
            }

            return accountWithToken;
        }

        public async Task<Account> LinkAccountWithSessionViaUsername(string username)
        {
            Account? accountWithToken = await _dataContext.Account.Include(t => t.Session).FirstOrDefaultAsync(u => u.Username == username);

            if (accountWithToken == null)
            {
                Log.Error($"A token cannot be paired with account with ID : {username}");
            }

            return accountWithToken;
        }

        public async Task<Account> GetSessionAndAccountViaSessionId(Guid sessionUnique)
        {
            Account? getAccountWithSession = await _dataContext.Account.Include(s => s.Session).FirstOrDefaultAsync(s => s.Session.SessionUnique == sessionUnique);

            if(getAccountWithSession == null)
            {
                Log.Warning($"Could not link a session with an account via session ID : {sessionUnique}");
            }

            return getAccountWithSession;
        }

        public async Task<Account> GetAccountWithSessionClientInstance(string username)
        {
            var thing = await _dataContext.Account
                .Include(s => s.Session)
                .ThenInclude(ci => ci.ClientInstance)
                .FirstOrDefaultAsync(u => u.Username == username);

            return thing;
        }
        public override Task SaveAsync()
        {
            return base.SaveAsync();
        }

    }
}
