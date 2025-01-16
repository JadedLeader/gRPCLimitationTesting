using gRPCToolFrontEnd.Interfaces;
using gRPCToolFrontEnd.Services;

namespace gRPCToolFrontEnd.Helpers
{
    public class ManyToManyUnaryHandler : IRequestReceiver
    {
        private readonly UnaryRequestService _unaryRequestService;

        public bool UnaryOrBatch { get; set; }

        public string FileSize { get; set; }

        public int BatchIterations { get; set; }

        public ManyToManyUnaryHandler(UnaryRequestService unaryRequestService, bool unaryOrBatch, string fileSize, int batchIterations)
        {
            _unaryRequestService = unaryRequestService;
            UnaryOrBatch = unaryOrBatch;
            FileSize = fileSize;
            BatchIterations = batchIterations;
        }

        public async Task ReceivingRequest()
        {
            if(UnaryOrBatch)
            {
                await _unaryRequestService.UnaryResponseIterativeAsync(FileSize);
            }
            else
            {
                await _unaryRequestService.UnaryBatchIterativeAsync(BatchIterations, FileSize);
            }
        }
    }
}
