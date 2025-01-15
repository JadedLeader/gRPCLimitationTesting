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

        private readonly DataContexts _dataContext;

        private DateTime _lastFetchedTime = DateTime.MinValue;

        public delayCalcRepo(DataContexts dataContexts) : base(dataContexts as DataContexts)
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

        public async Task<Dictionary<Guid, List<DelayCalc>>> GetNewDelays(Guid sessionUnique)
        {
            var clientInstances = await _dataContext.ClientInstance
                .Where(c => c.SessionUnique == sessionUnique)
                .Include(c => c.DelayCalcs)
                .ToListAsync();


            Log.Information($"fetched client instances with session unique {sessionUnique} ");

            var groupingItemsViaClientUnique = new Dictionary<Guid, List<DelayCalc>>();

            foreach(var clientInstance in clientInstances)
            {
                var newDelayCalcs = clientInstance.DelayCalcs
                    .Where(c => c.RecordCreation > _lastFetchedTime)
                    .ToList();

                if(newDelayCalcs.Any())
                {
                    groupingItemsViaClientUnique[clientInstance.ClientUnique] = newDelayCalcs;
                }
            }

            if(groupingItemsViaClientUnique.Any())
            {
                _lastFetchedTime = groupingItemsViaClientUnique
                    .SelectMany(kv => kv.Value)
                    .Max(dc => dc.RecordCreation);
            }

             
            return groupingItemsViaClientUnique;
                
        }

        public async Task Detach(DelayCalc entity)
        {
            var trackedEntity = _dataContext.ChangeTracker.Entries<DelayCalc>()
                .FirstOrDefault(e => e.Entity.messageId == entity.messageId);

            if (trackedEntity != null)
            {
                trackedEntity.State = EntityState.Detached;
            }
        }

        public async Task EmptyDelayCalcTable()
        {
            var allEntities = _dataContext.DelayCalc.ToList();

            _dataContext.DelayCalc.RemoveRange(allEntities);

           await SaveAsync();
        }
    }
}
