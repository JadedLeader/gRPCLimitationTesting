using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;

namespace ConfigurationStuff.Repositories;

public class LatencyMeasurementsRepo : ILatencyMeasurementsRepo
{

    private readonly IDataContexts _dataContexts;
    
    public LatencyMeasurementsRepo(IDataContexts dataContexts)
    {
        _dataContexts = dataContexts;   
    }

    public async Task AddToLatencyMeasurementsTable(LatencyMeasurements latencyMeasurement)
    {
        await _dataContexts.LatencyMeasurements.AddAsync(latencyMeasurement);
    }

    public void  RemoveLatencyFromMeasurementsTable(LatencyMeasurements latencyMeasurement)
    {
        _dataContexts.LatencyMeasurements.Remove(latencyMeasurement);
    }
    
}