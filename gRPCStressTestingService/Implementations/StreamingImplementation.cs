using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;

namespace gRPCStressTestingService.Implementations
{
    public class StreamingImplementation : StreamingLatency.StreamingLatencyBase
    {


        private readonly IStreamingLatencyService _streamingLatencyService; 
        public StreamingImplementation(IStreamingLatencyService streamingLatencyService)
        {
            _streamingLatencyService = streamingLatencyService; 
        }


        public override Task StreamingSingleBatchRequest(IAsyncStreamReader<StreamingBatchLatencyRequest> requestStream, IServerStreamWriter<StreamingBatchLatencyResponse> responseStream, ServerCallContext context)
        {
            return base.StreamingSingleBatchRequest(requestStream, responseStream, context);
        }

        public override async Task StreamingManySingleRequest(IAsyncStreamReader<StreamingManySingleLatencyRequest> requestStream, IServerStreamWriter<StreamingManySingleLatencyResponse> responseStream, ServerCallContext context)
        {
            await _streamingLatencyService.StreamingManySingleRequest(requestStream, responseStream, context);
        }

        public override async Task StreamingSingleRequest(IAsyncStreamReader<StreamingSingleLatencyRequest> requestStream, IServerStreamWriter<StreamingSingleLatencyResponse> responseStream, ServerCallContext context)
        {
            await _streamingLatencyService.StreamingSingleRequest(requestStream, responseStream, context);
        }

    }
}
