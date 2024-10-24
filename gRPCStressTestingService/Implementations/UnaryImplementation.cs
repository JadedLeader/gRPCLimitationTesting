using Grpc.Core;
using gRPCStressTestingService.proto;
using gRPCStressTestingService.Interfaces;
using System.Runtime.InteropServices;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.Interfaces.TimeStorage;
using Serilog;
using System.Dynamic;

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
                Log.Error($"{@calling} was null in the unary response implementation");       
            }

            return calling;

        }

        public override Task<BatchDataResponse> BatchUnaryResponse(BatchDataRequest request, ServerCallContext context)
        {
            var callingBatch = _unaryService.BatchUnaryResponse(request, context);

            if(callingBatch == null)
            {

                Log.Error($"{@callingBatch} was null in the unary batch response implementation");
            }

            return callingBatch;
        }

    }
}
