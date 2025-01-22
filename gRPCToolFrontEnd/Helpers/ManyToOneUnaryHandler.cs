using gRPCToolFrontEnd.Interfaces;
using gRPCToolFrontEnd.Services;

namespace gRPCToolFrontEnd.Helpers
{
    public class ManyToOneUnaryHandler : IRequestReceiver
    {
        private readonly UnaryRequestService _unaryRequestService;

        public bool UnaryOrBatch { get; set; }

        public Guid? ChannelUnique { get; set; }

        public string FileSize { get; set; }

        public int BatchIterations { get; set; }

        public ManyToOneUnaryHandler(UnaryRequestService unaryRequestService, bool unaryOrBatch, Guid? channelUnique, string fileSize, int batchIterations)
        {
            _unaryRequestService = unaryRequestService;
            UnaryOrBatch = unaryOrBatch;
            ChannelUnique = channelUnique;
            FileSize = fileSize;
            BatchIterations = batchIterations;
        }

        public async Task ReceivingRequest()
        {
            if(UnaryOrBatch)
            {
               // await _unaryRequestService.UnaryResponseAsync(ChannelUnique.Value, FileSize);

                await _unaryRequestService.UnaryResponseIterativeAsync(null, FileSize, BatchIterations);

            }
            else
            {
                //await _unaryRequestService.UnaryBatchResponseAsync(BatchIterations, ChannelUnique.Value, FileSize);

                await _unaryRequestService.UnaryBatchIterativeAsync(null, BatchIterations, FileSize);
            }
        }
    }
}
