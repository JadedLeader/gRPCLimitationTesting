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
    public class AuthTokenRepo : RepositoryAbstract<AuthToken>, IAuthTokenRepo
    {

        private readonly IDataContexts _dataContext;

        public AuthTokenRepo(IDataContexts dataContext) : base(dataContext as DbContext)
        {
            _dataContext = dataContext;
        }

        public override Task<AuthToken> AddToDbAsync(AuthToken entity)
        {
            return base.AddToDbAsync(entity);
        }

        public override Task<AuthToken> RemoveFromDbAsync(AuthToken entity)
        {
            return base.RemoveFromDbAsync(entity);
        }

        public override Task<AuthToken> UpdateDbAsync(AuthToken entity)
        {
            return base.UpdateDbAsync(entity);
        }

        public override Task<IEnumerable<AuthToken>> GetDbContent()
        {
            return base.GetDbContent();
        }

        public override async Task<AuthToken> GetRecordViaId(Guid? recordId)
        {
            AuthToken? retrievedToken = await _dataContext.AuthToken.FirstOrDefaultAsync(t => t.AuthUnique == recordId);

            if(retrievedToken == null)
            {
                Log.Error($"{retrievedToken} was null, no token with this ID could be found");
            }

            return retrievedToken;
        }

    }
}
