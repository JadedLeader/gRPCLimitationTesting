using Microsoft.Identity.Client;
using Serilog;
using Serilog.Sinks.File;

namespace gRPCToolFrontEnd.Services
{
    public class PresetService
    {


        private readonly UnaryRequestService _unaryRequestService;

        private readonly StreamingLatencyService _streamingLatencyService;

        private readonly MutliClientMultiChannelService _multiClientMultiChannelService;

        private readonly MutliChannelSingleClientService _multiChannelSingleClientService;
        public PresetService(UnaryRequestService unaryRequestService, StreamingLatencyService streamingLatencyService, MutliClientMultiChannelService multiClientMultiChannelService, 
            MutliChannelSingleClientService singleClientService)
        {
            _streamingLatencyService = streamingLatencyService;
            _unaryRequestService = unaryRequestService;
            _multiClientMultiChannelService = multiClientMultiChannelService;
            _multiChannelSingleClientService = singleClientService;
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
        public async Task LowStress(bool lowStressRunning, int amountOfChannels)
        {
                string fileSize = "small";

                await _multiChannelSingleClientService.UnaryClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

                await _multiChannelSingleClientService.UnaryBatchClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

                await _multiChannelSingleClientService.StreamingClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

                await _multiChannelSingleClientService.StreamingBatchClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

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
        public async Task MediumStress(bool mediumStressRunning, int amountOfChannels)
        {
            
               string fileSize = "small";


              await _multiChannelSingleClientService.UnaryClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

              await _multiChannelSingleClientService.UnaryBatchClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

              await _multiChannelSingleClientService.StreamingClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

              await _multiChannelSingleClientService.StreamingBatchClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

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
        public async Task HighStress(bool highStressRunning, int amountOfChannels)
        {
                int amountOfRequests = 0;

                string fileSize = "small";

                await _multiChannelSingleClientService.UnaryClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

                await _multiChannelSingleClientService.UnaryBatchClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

                await _multiChannelSingleClientService.StreamingClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);

                await _multiChannelSingleClientService.StreamingBatchClientToSingleChannelAllocation(fileSize, amountOfChannels, 1);   
        }

        public async Task MutliClientLowStress(bool lowStressRunning, int amountOfChannels)
        {
            string fileSize = "small";

                await _multiClientMultiChannelService.UnaryBatchClientToChannelAllocation(5, fileSize, 1, amountOfChannels);

                await _multiClientMultiChannelService.StreamingClientToChannelAllocation(5, 1, fileSize, amountOfChannels);

                await _multiClientMultiChannelService.StreamingBatchClientToChannelAllocation(5, 1, fileSize, amountOfChannels);

                await _multiClientMultiChannelService.UnaryClientToChannelAllocation(5, fileSize, 1, amountOfChannels);

            Log.Information($"Low stress mutli-client has stopped running");
        }

        public async Task MutliClientMediumStress(bool mediumStressRunning, int amountOfChannels)
        {
                string fileSize = "small";

                await _multiClientMultiChannelService.StreamingClientToChannelAllocation(10, 1, fileSize, amountOfChannels);

                await _multiClientMultiChannelService.StreamingBatchClientToChannelAllocation(10, 1, fileSize, amountOfChannels);

                await _multiClientMultiChannelService.UnaryClientToChannelAllocation(10, fileSize, 1, amountOfChannels);

                await _multiClientMultiChannelService.UnaryBatchClientToChannelAllocation(10, fileSize, 1, amountOfChannels);

                Log.Information($"Medium stress has stopped running");
          
        }

        public async Task MultiClientHighStress(bool highStressRunning, int amountOfChannels)
        {
        
            string fileSize = "small";

            await _multiClientMultiChannelService.StreamingClientToChannelAllocation(15, 1, fileSize, amountOfChannels);

            await _multiClientMultiChannelService.StreamingBatchClientToChannelAllocation(15, 1, fileSize, amountOfChannels);

            await _multiClientMultiChannelService.UnaryClientToChannelAllocation(15, fileSize, 1, amountOfChannels);

            await _multiClientMultiChannelService.UnaryBatchClientToChannelAllocation(15, fileSize, 1, amountOfChannels);

            
        }
    }
}
