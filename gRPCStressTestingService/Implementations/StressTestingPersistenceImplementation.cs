using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService.protos;

namespace gRPCStressTestingService.Implementations;

public class StressTestingPersistenceImplementation : StressTestingPersistence.StressTestingPersistenceBase
{
    
    private readonly IStressTestingPersistenceService _stressTestingPersistenceService;
    
    public StressTestingPersistenceImplementation(IStressTestingPersistenceService stressTestingPersistenceService)
    {
        _stressTestingPersistenceService = stressTestingPersistenceService; 
    }
    
    public override async Task<SaveSessionPointResponse> SaveSession(IAsyncStreamReader<SaveSessionPointRequest> requestStream, ServerCallContext context)
    {
       SaveSessionPointResponse savingSessionPoint = await _stressTestingPersistenceService.SaveSession(requestStream, context);

       if (savingSessionPoint.Success == false)
       {
           throw new RpcException(new Status(StatusCode.Internal,
               $"Internal server failure when saving the session point"));
       }

       return savingSessionPoint;
    }
}