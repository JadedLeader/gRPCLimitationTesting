using DbManagerWorkerService.Interfaces.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationStuff.DbModels;

namespace ConfigurationStuff.Interfaces.Repos
{
    public interface IDelayCalcRepo
    {


        public Task<DelayCalc> AddToDbAsync(DelayCalc entity);


        public Task<DelayCalc> RemoveFromDbAsync(DelayCalc entity);


        public Task<IEnumerable<DelayCalc>> GetDbContent();

        public Task<Dictionary<Guid, List<DelayCalc>>> GetAllDelays();

        public Task<Dictionary<Guid, List<DelayCalc>>> GetNewDelays();

        public Task EmptyDelayCalcTable();

        public Task SaveAsync();


    }
}
