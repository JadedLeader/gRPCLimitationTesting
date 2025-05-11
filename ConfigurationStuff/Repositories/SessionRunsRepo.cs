using ConfigurationStuff.Abstracts;
using ConfigurationStuff.DbContexts;
using ConfigurationStuff.DbModels;
using ConfigurationStuff.DTO;
using ConfigurationStuff.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<SessionRunInformation>> GetSessionRunsViaSesionUnique(Guid sessionUnique)
    {
        var sessionRuns = await _dataContexts.SessionRuns
            .Where(x => x.SessionUnique == sessionUnique)
            .Select(x => new SessionRunInformation
            {
                PresetName = x.PresetName,
                SessionsRunId = x.SessionsRunId,
                OverarchingPresetName = x.OverarchingPresetName,
                
            })
            .ToListAsync();

        if (sessionRuns.Count == 0)
        {
            return new List<SessionRunInformation>();
        }

        return sessionRuns;

    }

    public async Task<List<string>> GetSessionRunIds(string overarchingPresetName)
    {
        var sessionRunIdsForOverarching = _dataContexts.SessionRuns
            .Where(x => x.OverarchingPresetName == overarchingPresetName)
            .Select(x => x.SessionsRunId.ToString())
            .ToList();

        if (sessionRunIdsForOverarching.Count == 0)
        {
            return new List<string>();  
        }
        
        return sessionRunIdsForOverarching;
    }
    
    public override Task SaveAsync()
    {
        return base.SaveAsync();
    }
}