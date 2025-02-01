using gRPCToolFrontEnd.Interfaces;
using gRPCToolFrontEnd.Services;

namespace gRPCToolFrontEnd.Helpers
{
    public class ManyToOneStreamingHandler : IRequestReceiver
    {

        private readonly StreamingLatencyService _streamingLatencyService;

        public bool UnaryOrBatch { get; set; }

        public Guid? ChannelUnique { get; set; }

        public int RequestsInBatch { get; set; }

        public string FileSize { get; set; }

        public ManyToOneStreamingHandler(StreamingLatencyService streamingLatencyService, bool unaryOrBatch, Guid? channelUnique, int requestsInBatch, string fileSize)
        {
            _streamingLatencyService = streamingLatencyService;
            UnaryOrBatch = unaryOrBatch;
            ChannelUnique = channelUnique;
            RequestsInBatch = requestsInBatch;
            FileSize = fileSize;
        }

        public async Task ReceivingRequest()
        {
            if(UnaryOrBatch)
            {
                await _streamingLatencyService.CreateManySingleStreamingRequests(null, null, RequestsInBatch, FileSize);
            }
            else
            {
                await _streamingLatencyService.CreateManyStreamingBatchRequest(null, RequestsInBatch, FileSize);


            }
        }
    }
}
