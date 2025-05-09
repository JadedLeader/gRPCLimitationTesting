using ConfigurationStuff.DbModels;

namespace ConfigurationStuff.Interfaces.Repos;

public interface ISessionRunsRepo
{
    public Task<SessionRuns> AddToDbAsync(SessionRuns entity);

    public Task SaveAsync();
}