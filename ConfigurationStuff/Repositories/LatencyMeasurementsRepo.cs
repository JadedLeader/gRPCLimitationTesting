using ConfigurationStuff.Abstracts;
using ConfigurationStuff.DbContexts;
using ConfigurationStuff.DbModels;
using ConfigurationStuff.DTO;
using ConfigurationStuff.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationStuff.Repositories;

public class LatencyMeasurementsRepo :  RepositoryAbstract<LatencyMeasurements>,ILatencyMeasurementsRepo
{

    private readonly IDataContexts _dataContexts;
    
    public LatencyMeasurementsRepo(IDataContexts dataContext) : base(dataContext as DataContexts)
    {
        _dataContexts = dataContext;   
    }


    public override Task<LatencyMeasurements> AddToDbAsync(LatencyMeasurements entity)
    {
        return base.AddToDbAsync(entity);
    }

    public override Task<LatencyMeasurements> GetRecordViaId(Guid? recordId)
    {
        throw new NotImplementedException();
    }

    public override Task SaveAsync()
    {
        return base.SaveAsync();
    }

    public async Task<List<LatencyMeasurementInformation>> GetLatencyMeasurementsViaSessionRunId(string sessionRunId)
    {
        List<LatencyMeasurementInformation> latencies = await _dataContexts.LatencyMeasurements
            .Where(x => x.SessionRuns.SessionsRunId == Guid.Parse(sessionRunId))
            .Select(x => new LatencyMeasurementInformation()
            {
                TestType = x.TestType,
                Latency = x.Latency,
                ClientType = x.ClientType, 
                StressLevel = x.StressLevel
            })
            .ToListAsync();

        if (latencies.Count == 0)
        {
            return new List<LatencyMeasurementInformation>();
        }
        
        return latencies;
    }

    public async Task<List<LatencyMeasurementInformation>> GetLatencyMeasurementsViaSessionRunId(List<string> sessionRunIds)
    {
        List<LatencyMeasurementInformation> latencies = new List<LatencyMeasurementInformation>();
        
        foreach (var sessionRunId in sessionRunIds)
        {
           List<LatencyMeasurementInformation> batch = await  GetLatencyMeasurementsViaSessionRunId(sessionRunId);
           
           latencies.AddRange(batch);
        }

        if (latencies.Count == 0)
        {
            return new List<LatencyMeasurementInformation>();   
        }
        
        return latencies;
    }
    
    
}