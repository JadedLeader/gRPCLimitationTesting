using gRPCToolFrontEnd.Interfaces;
using gRPCToolFrontEnd.Services;

namespace gRPCToolFrontEnd.Helpers
{
    public class OneToOneUnaryHandler : IRequestReceiver
    {

     
        private UnaryRequestService _unaryService;

        public bool UnaryOrBatch { get; set; }    

        public Guid? ChannelUnique { get; set; } 

        public string FileSize { get; set; }

        public string ClientUnique { get; set; }    

        public int IterationsInBatch { get; set; }

        public OneToOneUnaryHandler( UnaryRequestService unaryService, bool unaryOrBatch, Guid? channelUnique, string fileSize, 
            string clientUnique, int iterationsInBatch )
        {
            _unaryService = unaryService;
            UnaryOrBatch = unaryOrBatch;
            ChannelUnique = channelUnique;
            FileSize = fileSize;
            ClientUnique = clientUnique;
            IterationsInBatch = iterationsInBatch;
        }
        public async Task ReceivingRequest()
        {
            if(UnaryOrBatch)
            {
                await _unaryService.UnaryResponseAsync(ChannelUnique.Value, FileSize);
            }
            else
            {
                await _unaryService.UnaryBatchResponseAsync(IterationsInBatch, ChannelUnique.Value, FileSize);
            }
        }
    }
}
