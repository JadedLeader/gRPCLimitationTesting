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

        public Task<SessionRepo> AddToDbAsync(SessionRepo entity);

        public Task<SessionRepo> RemoveFromDbAsync(SessionRepo entity);

        public Task<IEnumerable<SessionRepo>> GetDbContent();
       
    }
}
