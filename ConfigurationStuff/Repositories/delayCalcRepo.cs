using ConfigurationStuff.Abstracts;
using DbManagerWorkerService.Interfaces.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using ConfigurationStuff.DbContexts;

namespace DbManagerWorkerService.Repositories
{
    public class delayCalcRepo : RepositoryAbstract<DelayCalc>, IDelayCalcRepo
    {

        private readonly IDataContexts _dataContext;

        public delayCalcRepo(IDataContexts dataContexts) : base(dataContexts as DataContexts)
        {
            _dataContext = dataContexts;
        }

        public override Task<DelayCalc> AddToDbAsync(DelayCalc entity)
        {
            return base.AddToDbAsync(entity);
        }

        public override Task<DelayCalc> RemoveFromDbAsync(DelayCalc entity)
        {
            return base.RemoveFromDbAsync(entity);
        }

        public override Task<DelayCalc> UpdateDbAsync(DelayCalc entity)
        {
            return base.UpdateDbAsync(entity);
        }

        public override Task<IEnumerable<DelayCalc>> GetDbContent()
        {
            return base.GetDbContent();
        }

        public override async Task<DelayCalc> GetRecordViaId(Guid? recordId)
        {
            throw new NotImplementedException();
        }

        public override Task SaveAsync()
        {
            return base.SaveAsync();
        }
    }
}
