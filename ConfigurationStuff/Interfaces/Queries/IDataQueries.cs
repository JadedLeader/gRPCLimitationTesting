using ConfigurationStuff.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationStuff.Interfaces.Queries
{
    public interface IDataQueries
    {

        public Task<List<ClientInstance>> GetAllClientInstancesWithMessages();

    }
}
