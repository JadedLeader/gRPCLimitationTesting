using ConfigurationStuff.DbModels;

namespace ConfigurationStuff.Interfaces.Repos;

public interface ILatencyMeasurementsRepo
{
    public Task AddToLatencyMeasurementsTable(LatencyMeasurements latencyMeasurement);

    public void RemoveLatencyFromMeasurementsTable(LatencyMeasurements latencyMeasurement);
}