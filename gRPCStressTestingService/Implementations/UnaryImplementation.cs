using Grpc.Core;
using gRPCStressTestingService.proto;
using gRPCStressTestingService.Interfaces;
using System.Runtime.InteropServices;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.Interfaces.TimeStorage;

namespace gRPCStressTestingService.Implementations
{
    public class UnaryImplementation : Unary.UnaryBase
    {

        private readonly IUnaryService _unaryService;
        private readonly IRequestResponseTimeStorage _requestResponseTimeStorage;

        public UnaryImplementation(IUnaryService unaryService, IRequestResponseTimeStorage requestResponseTimeStorage)
        {
            _unaryService = unaryService;
            _requestResponseTimeStorage = requestResponseTimeStorage;
        }

        public override Task<DataResponse> UnaryResponse(DataRequest request, ServerCallContext context)
        {
            var calling = _unaryService.UnaryResponse(request, context);

            if (calling == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "The details for the unary response could not be found"));    
            }

            return calling;

        }

    }
}
