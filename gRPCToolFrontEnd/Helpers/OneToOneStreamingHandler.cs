using gRPCToolFrontEnd.Interfaces;
using gRPCToolFrontEnd.Services;
using Serilog;

namespace gRPCToolFrontEnd.Helpers
{
    public class OneToOneStreamingHandler : IRequestReceiver
    {

        private readonly StreamingLatencyService _streamingLatencyService;

        public bool UnaryOrBatch { get; set; }

        public Guid? ChannelUnique { get; set; }

        public string FileSize { get; set; }

        public int RequestsInBatch { get; set; }


        public OneToOneStreamingHandler(StreamingLatencyService streamingLatencyService, bool unaryOrBatch, Guid? channelUnique, string fileSize, int requestsInBatch)
        {
            _streamingLatencyService = streamingLatencyService;
            UnaryOrBatch = unaryOrBatch;
            ChannelUnique = channelUnique;
            FileSize = fileSize;
            RequestsInBatch = requestsInBatch;

        }

        public async Task ReceivingRequest()
        {
            Log.Information($"One to one streaming request detected");

            if(UnaryOrBatch)
            {
                await _streamingLatencyService.SendingSingleUnaryRequestStream(ChannelUnique.Value, FileSize);
            }
            else
            {
                await _streamingLatencyService.CreateSingleStreamingBatchRequest(ChannelUnique.Value, RequestsInBatch, FileSize);
            }
        }
    }
}
