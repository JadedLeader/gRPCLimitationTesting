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
       

        public UnaryImplementation(IUnaryService unaryService)
        {
            _unaryService = unaryService;
            
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

        public override Task<BatchDataResponse> BatchUnaryResponse(BatchDataRequest request, ServerCallContext context)
        {
            return base.BatchUnaryResponse(request, context);
        }

    }
}
