using Serilog;

namespace gRPCToolFrontEnd.Services
{
    public class PresetService
    {


        private readonly UnaryRequestService _unaryRequestService;

        private readonly StreamingLatencyService _streamingLatencyService;
        public PresetService(UnaryRequestService unaryRequestService, StreamingLatencyService streamingLatencyService)
        {
            _streamingLatencyService = streamingLatencyService;
            _unaryRequestService = unaryRequestService;
        }


        /// <summary>
        /// A low stress definition is only using many : one requests with a "small" payload 
        /// Small payloads are 1MB of content per request payload
        /// Many to one to simulate many clients becoming active and sending a single request 
        /// This will entail streaming and latency versions of all single request types, being streaming/unary single request and streaming/unary single batch request with one batch iteration within
        /// This might even have a delay before the next one can run to produce even less stress on the system
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task LowStress(bool lowStressRunning)
        {

            string fileSize = "small";

            while(lowStressRunning)
            {
                await _unaryRequestService.UnaryResponseIterativeAsync(null, fileSize);

                await _unaryRequestService.UnaryBatchIterativeAsync(null, 1, fileSize);

                await _streamingLatencyService.CreateManySingleStreamingRequests(null, 1, fileSize);

                await _streamingLatencyService.CreateManyStreamingBatchRequest(null, 1, fileSize);
            }

            Log.Information($"Low stress has stopped running");

        }

        /// <summary>
        /// Medium stress definition is using many : many requests with a small - medium payload
        /// Small payloads are 1MB of content per request payload
        /// Medium payloads are 30MB of content per request payload 
        /// This will entail both streaming/unary requests with their single variants of single request and single batch request with 3 batch iterations within the batch requests
        /// This will have a shorter delay than low stress 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task MediumStress(bool mediumStressRunning)
        {
            //we need a way to randomise 

            while(mediumStressRunning)
            {

                string fileSize = "";

                Random random = new Random();

                int number = random.Next(2);

                if (number == 1)
                {
                    fileSize = "small";
                }
                else if (number == 2)
                {
                    fileSize = "medium";
                }
                else
                {
                    Log.Information($"Something went wrong with the random number generation for the medium stress");
                }

                await _unaryRequestService.UnaryResponseIterativeAsync(null, fileSize);

                await _unaryRequestService.UnaryBatchIterativeAsync(null, 3, fileSize);

                await _streamingLatencyService.CreateManySingleStreamingRequests(null, 3, fileSize);

                await _streamingLatencyService.CreateManyStreamingBatchRequest(null, 3, fileSize);

            }

            Log.Information($"Medium stress has stopped running");
               
        }

        /// <summary>
        /// High stress defintion is using many : many requests with varying payloads, all the way from small -> large
        /// Small payloads are 1MB of content per request payload
        /// Medium payloadsd are 30MB of content per request payload
        /// Large payloads are 100MB of content per request payload (this is currently the max that the gRPC endpoints can handle with kestrel)
        /// This will entail both streaming/unary requests with iterative versions thrown into the mix alongside single variants
        /// This will have no delay
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task HighStress()
        {
            throw new NotImplementedException();
        }

    }
}
