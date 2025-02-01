using gRPCToolFrontEnd.Interfaces;
using gRPCToolFrontEnd.Services;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace gRPCToolFrontEnd.Helpers
{
    public class ManyToManyStreamingHandler : IRequestReceiver
    {

        private readonly StreamingLatencyService _streamingLatencyService;

        public bool UnaryOrBatch { get; set; }

        public int AmountOfRequests { get; set; }

        public string FileSize { get; set; }

        public int RequestsInBatch { get; set; }

        public ManyToManyStreamingHandler(StreamingLatencyService streamingLatencyService, bool unaryOrBatch, int amountOfRequests, string fileSize, int requestsInBatch)
        {
            _streamingLatencyService = streamingLatencyService;
            UnaryOrBatch = unaryOrBatch;
            AmountOfRequests = amountOfRequests;
            FileSize = fileSize;
            RequestsInBatch = requestsInBatch;
        }

        public async Task ReceivingRequest()
        {
            Log.Information("many to many streaming request detected");

            if(UnaryOrBatch)
            {
                await _streamingLatencyService.CreateManySingleStreamingRequests(null, null, AmountOfRequests, FileSize);
            }
            else
            {
                await _streamingLatencyService.CreateManyStreamingBatchRequest(null, RequestsInBatch, FileSize);
            }
        }
    }
}
