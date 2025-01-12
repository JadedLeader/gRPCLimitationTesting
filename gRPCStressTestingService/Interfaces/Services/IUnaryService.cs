using Grpc.Core;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IUnaryService
    {

        public Task<DataResponse> UnaryResponse(DataRequest request, ServerCallContext context);

        public Task<BatchDataResponse> BatchUnaryResponse(BatchDataRequest request, ServerCallContext context);

    }
}
