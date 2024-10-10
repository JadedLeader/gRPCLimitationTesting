using Grpc.Core;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.Interfaces
{
    public interface IUnaryService
    {

        public Task<DataResponse> UnaryResponse(DataRequest request, ServerCallContext context);

    }
}
