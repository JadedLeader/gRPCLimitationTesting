using ConfigurationStuff.DbModels;

namespace ConfigurationStuff.Interfaces.Repos;

public interface ILatencyMeasurementsRepo
{
    public Task<LatencyMeasurements> AddToDbAsync(LatencyMeasurements entity);

    public Task SaveAsync();
}