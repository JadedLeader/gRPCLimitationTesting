using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;

namespace ConfigurationStuff.Repositories;

public class SessionRunsRepo : ISessionRunsRepo
{
    
    private readonly IDataContexts _dataContexts;
    
    public SessionRunsRepo(IDataContexts dataContexts)
    {
        _dataContexts = dataContexts;
    }


    public async Task AddSessionsToSessionsRunsTable(SessionRuns sessionRuns)
    {
        await _dataContexts.SessionRuns.AddAsync(sessionRuns); 
    }
    
}