using ConfigurationStuff.DbModels;
using ConfigurationStuff.DTO;

namespace ConfigurationStuff.Interfaces.Repos;

public interface ILatencyMeasurementsRepo
{
    public Task<LatencyMeasurements> AddToDbAsync(LatencyMeasurements entity);

    public Task SaveAsync();

    public Task<List<LatencyMeasurementInformation>> GetLatencyMeasurementsViaSessionRunId(string sessionRunId);

    public Task<List<LatencyMeasurementInformation>> GetLatencyMeasurementsViaSessionRunId(List<string> sessionRunIds);
}