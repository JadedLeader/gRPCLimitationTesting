using ConfigurationStuff.Abstracts;
using ConfigurationStuff.DbContexts;
using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;

namespace ConfigurationStuff.Repositories;

public class SessionRunsRepo : RepositoryAbstract<SessionRuns>,  ISessionRunsRepo
{
    
    private readonly IDataContexts _dataContexts;
    
    public SessionRunsRepo(IDataContexts dataContext) : base(dataContext as DataContexts)
    {
        _dataContexts = dataContext;
    }


    public override Task<SessionRuns> AddToDbAsync(SessionRuns entity)
    {
        return base.AddToDbAsync(entity);
    }

    public override Task<SessionRuns> GetRecordViaId(Guid? recordId)
    {
        throw new NotImplementedException();
    }

    public override Task SaveAsync()
    {
        return base.SaveAsync();
    }
}