using ConfigurationStuff.DbModels;
using ConfigurationStuff.DTO;

namespace ConfigurationStuff.Interfaces.Repos;

public interface ISessionRunsRepo
{
    public Task<SessionRuns> AddToDbAsync(SessionRuns entity);

    public Task SaveAsync();

    public Task<List<SessionRunInformation>> GetSessionRunsViaSesionUnique(Guid sessionUnique);
}