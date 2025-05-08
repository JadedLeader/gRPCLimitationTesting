using ConfigurationStuff.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;

namespace gRPCStressTestingService.Services;

public class StressTestingPersistenceService : IStressTestingPersistenceService
{
    
    private readonly ISessionRunsRepo _sessionRunsRepo;
    private readonly ILatencyMeasurementsRepo _latencyMeasurementsRepo;
    
    public StressTestingPersistenceService(ISessionRunsRepo sessionRunsRepo, ILatencyMeasurementsRepo latencyMeasurementsRepo)
    {
        _sessionRunsRepo = sessionRunsRepo;
        _latencyMeasurementsRepo = latencyMeasurementsRepo;
    }

    public async Task<SaveSessionPointResponse> SaveSession(IAsyncStreamReader<SaveSessionPointRequest> requestStream, ServerCallContext context)
    {
        //so this needs to be in charge of saving to the database 

        throw new NotImplementedException();
        
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await foreach (var response in requestStream.ReadAllAsync())
            {
                if (response.TestType == TestType.UnarySingle)
                {
                    
                }
                else if (response.TestType == TestType.UnaryBatch)
                {
                    
                }
                else if (response.TestType == TestType.StreamingSingle)
                {
                    
                }
                else if (response.TestType == TestType.StreamingBatch)
                {
                    
                }
            }
        }

        
    }
    
    
}