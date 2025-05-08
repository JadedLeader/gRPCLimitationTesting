using ConfigurationStuff.DbModels;

namespace ConfigurationStuff.Interfaces.Repos;

public interface ISessionRunsRepo
{
    public Task AddSessionsToSessionsRunsTable(SessionRuns sessionRuns);
}