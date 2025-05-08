using Grpc.Core;
using gRPCStressTestingService.protos;

namespace gRPCStressTestingService.Implementations;

public class StressTestingPersistenceImplementation : StressTestingPersistence.StressTestingPersistenceBase
{

    public StressTestingPersistenceImplementation()
    {
        
    }
    
    public override async Task<SaveSessionPointResponse> SaveSession(IAsyncStreamReader<SaveSessionPointRequest> requestStream, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}