using DbManagerWorkerService.Abstracts;
using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManagerWorkerService.Repositories
{
    public class delayCalcRepo : RepositoryAbstract<DelayCalc>, IDelayCalcRepo
    {

        private readonly IDataContexts _dataContext;

        public delayCalcRepo(IDataContexts dataContexts) : base(dataContexts as DbContext)
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
    }
}
