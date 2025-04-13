namespace gRPCToolFrontEnd.Services
{
    public class MutliChannelSingleClientService
    {


        private readonly UnaryRequestService _unaryRequestService;
        private readonly StreamingLatencyService _streamingLatencyService;

        public MutliChannelSingleClientService(UnaryRequestService unaryRequestService, StreamingLatencyService streamingLatencyService)
        {
            _unaryRequestService = unaryRequestService;
            _streamingLatencyService = streamingLatencyService;
        }

        public async Task UnaryClientToSingleChannelAllocation( string fileSize, int amountOfChannels, int amountOfRequests)
        {


            List<Task> tasks = new List<Task>();

            tasks.Add(Task.Run(async () =>
            {
                await _unaryRequestService.UnaryResponseIterativeAsync(true, fileSize, amountOfRequests, amountOfChannels);
            }));

             
              
        }

        public async Task UnaryBatchClientToSingleChannelAllocation( string fileSize, int amountOfChannels, int amountOfRequests)
        {

            List<Task> tasks = new List<Task>();

            
                tasks.Add(Task.Run(async () =>
                {
                    await _unaryRequestService.UnaryBatchIterativeAsync(true, amountOfRequests, fileSize, amountOfChannels);
                }));

              

        }

        public async Task StreamingClientToSingleChannelAllocation( string fileSize, int amountOfChannels, int amountOfRequests)
        {
            List<Task> tasks = new List<Task>();

           
                tasks.Add(Task.Run(async () =>
                {
                    await _streamingLatencyService.CreateManySingleStreamingRequests(null, true, amountOfRequests, fileSize, amountOfChannels);
                }));

             

        }

        public async Task StreamingBatchClientToSingleChannelAllocation( string fileSize, int amountOfChannels, int amountOfRequests)
        {
            List<Task> tasks = new List<Task>();

           

           
                tasks.Add(Task.Run(async () =>
                {
                    await _streamingLatencyService.CreateManyStreamingBatchRequest(true, amountOfRequests, fileSize, amountOfChannels);
                }));


              
    
            
        }

    }
}
