using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Queries;
using DbManagerWorkerService.Interfaces.DataContext;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationStuff.Queries
{
    public class DataQueries : IDataQueries
    {
        private readonly IDataContexts _dataContext;
        public DataQueries(IDataContexts dataContext)
        {
            _dataContext = dataContext;
        }


        public async Task<List<ClientInstance>> GetAllClientInstancesWithMessages()
        {
            var thing =  await _dataContext.ClientInstance.Include(x => x.DelayCalcs).ToListAsync();

            if(thing.Count == 0)
            {
                Log.Warning($"Client instances cannot be linked with delay calcs for the query");
            }

            return thing;
        }

    }
}
