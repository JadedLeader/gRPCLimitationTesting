using gRPCToolFrontEnd.Interfaces;
using gRPCToolFrontEnd.Services;

namespace gRPCToolFrontEnd.Helpers
{
    public class OneToManyUnaryHandler : IRequestReceiver
    {
        private readonly UnaryRequestService _unaryRequestService;

        public bool UnaryOrBatch { get; set; }

        public Guid? ChannelUnique { get; set; }

        public string FileSize { get; set; }

        public int BatchIterations { get; set; }

        public OneToManyUnaryHandler(UnaryRequestService unaryRequestService, bool unaryOrBatch, Guid? channelUnique, string fileSize, int batchIterations)
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
                await _unaryRequestService.UnaryResponseIterativeAsync(ChannelUnique.Value, FileSize);
            }
            else
            {
                await _unaryRequestService.UnaryBatchIterativeAsync(ChannelUnique.Value, BatchIterations, FileSize);
            }
        }
    }
}
