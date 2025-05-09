using ConfigurationStuff.Abstracts;
using ConfigurationStuff.DbContexts;
using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;

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
    
    
}