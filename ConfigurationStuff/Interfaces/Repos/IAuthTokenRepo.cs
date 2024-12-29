using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationStuff.DbModels;

namespace ConfigurationStuff.Interfaces.Repos
{
    public interface IAuthTokenRepo
    {
        public Task<AuthToken> AddToDbAsync(AuthToken entity);

        public Task<AuthToken> RemoveFromDbAsync(AuthToken entity);

        public Task<IEnumerable<AuthToken>> GetDbContent();

        public Task<AuthToken> GetRecordViaId(Guid? recordId);

        public Task<AuthToken> UpdateDbAsync(AuthToken entity);

        public Task SaveAsync();



    }
}
