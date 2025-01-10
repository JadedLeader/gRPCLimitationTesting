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
using Serilog;
using Microsoft.Identity.Client;

namespace DbManagerWorkerService.Repositories
{
    public class delayCalcRepo : RepositoryAbstract<DelayCalc>, IDelayCalcRepo
    {

        private readonly IDataContexts _dataContext;

        private DateTime _lastFetchedTime = DateTime.MinValue;

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

        public async Task<Dictionary<Guid, List<DelayCalc>>> GetAllDelays()
        {
            var thing = _dataContext.DelayCalc
                        .Where(calc => calc.ClientUnique.HasValue)
                        .GroupBy(calc => calc.ClientUnique.Value)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Distinct().ToList()
                            );


            return thing;
        }

        public async Task<Dictionary<Guid, List<DelayCalc>>> GetNewDelays()
        {
            var newDelays = await _dataContext.DelayCalc
                 .Where(calc => calc.ClientUnique.HasValue && calc.RecordCreation > _lastFetchedTime).ToListAsync();

            Log.Information($"last fetched time was {_lastFetchedTime}");

             if(!newDelays.Any())
             {
                 return new Dictionary<Guid, List<DelayCalc>> ();
             }

             var setMaxTime = newDelays.Max(x => x.RecordCreation);

             _lastFetchedTime = setMaxTime;

             var groupingItemViaClientUnique = newDelays
                 .GroupBy(calc => calc.ClientUnique.Value)
                 .ToDictionary(
                 group => group.Key,
                 group => group.ToList()
                 );


             return groupingItemViaClientUnique; 
            
                

        }

        public async Task EmptyDelayCalcTable()
        {
            var allEntities = _dataContext.DelayCalc.ToList();

            _dataContext.DelayCalc.RemoveRange(allEntities);

           await SaveAsync();
        }
    }
}
