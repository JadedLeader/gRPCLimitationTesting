using gRPCToolFrontEnd.Interfaces;
using gRPCToolFrontEnd.Services;

namespace gRPCToolFrontEnd.Helpers
{
    public class OneToManyStreamingHandler : IRequestReceiver
    {
        private readonly StreamingLatencyService _streamingLatencyService;

        public bool UnaryOrBatch { get; set ; }

        public int RequestsInBatch { get; set; }

        public string FileSize { get; set; }

        public Guid? ChannelUnique { get; set; }

        public int AmountOfRequests { get ; set; }

        public OneToManyStreamingHandler(StreamingLatencyService streamingLatencyService, bool unaryOrBatch, int requestsInBatch, string fileSize, Guid? channelUnique,
            int amountOfRequests)
        {
            _streamingLatencyService = streamingLatencyService;
            UnaryOrBatch = unaryOrBatch;
            RequestsInBatch = requestsInBatch;
            FileSize = fileSize;
            ChannelUnique = channelUnique;
            AmountOfRequests = amountOfRequests;
            
        }

        public async Task ReceivingRequest()
        {
            if(UnaryOrBatch)
            {
                await _streamingLatencyService.CreateManySingleStreamingRequests(ChannelUnique.Value, AmountOfRequests, FileSize);
            }
            else
            {
                await _streamingLatencyService.CreateManyStreamingBatchRequest(ChannelUnique.Value, RequestsInBatch, FileSize);
            }
        }
    }
}
