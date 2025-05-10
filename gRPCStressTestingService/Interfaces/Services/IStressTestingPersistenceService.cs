using ConfigurationStuff.DbModels;
using Grpc.Core;

namespace gRPCStressTestingService.Interfaces.Services;

public interface IStressTestingPersistenceService
{
    public Task<SaveSessionPointResponse> SaveSession(IAsyncStreamReader<SaveSessionPointRequest> requestStream,
        ServerCallContext context);

    public Task StreamSessionRuns(StreamSessionRunRequest request,
        IServerStreamWriter<StreamSessionRunResponse> responseStream, ServerCallContext context);

}