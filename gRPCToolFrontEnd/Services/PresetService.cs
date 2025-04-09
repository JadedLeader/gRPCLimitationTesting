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
        public PresetService(UnaryRequestService unaryRequestService, StreamingLatencyService streamingLatencyService, MutliClientMultiChannelService multiClientMultiChannelService)
        {
            _streamingLatencyService = streamingLatencyService;
            _unaryRequestService = unaryRequestService;
            _multiClientMultiChannelService = multiClientMultiChannelService;
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

                 var t1 = _unaryRequestService.UnaryBatchIterativeAsync(null, 1, fileSize);

                 var t2 = _streamingLatencyService.CreateManySingleStreamingRequests(null, null, 1, fileSize);

                 var t3 = _streamingLatencyService.CreateManyStreamingBatchRequest(null, 1, fileSize);
                
                 var t4 = _unaryRequestService.UnaryResponseIterativeAsync(null, fileSize, 1);

                 await Task.WhenAll(t1,t2,t3,t4); 

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

                Log.Information($"Number chose for medium stress : {number}");

                if (number == 0)
                {
                    fileSize = "small";
                }
                else if (number == 1)
                {
                    fileSize = "medium";
                }
                else
                {
                    Log.Information($"Something went wrong with the random number generation for the medium stress");
                }


                 var t1 = _unaryRequestService.UnaryBatchIterativeAsync(null, 3, fileSize);

                 var t2 = _streamingLatencyService.CreateManySingleStreamingRequests(null, null, 3, fileSize);

                 var t3 = _streamingLatencyService.CreateManyStreamingBatchRequest(null, 3, fileSize);

                 var t4 = _unaryRequestService.UnaryResponseIterativeAsync(null, fileSize, 3);

                 await Task.WhenAll(t1, t2, t3, t4); 

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
        public async Task HighStress(bool highStressRunning)
        {
            while (highStressRunning)
            {

                int amountOfRequests = 0;

                string fileSize = "";

                Random random = new Random();

                int number = random.Next(3);

                if (number == 0)
                {
                    fileSize = "small";

                    amountOfRequests = 30;
                }
                else if (number == 1)
                {
                    fileSize = "medium";

                    amountOfRequests = 3;
                }
                else if (number == 2)
                {
                    fileSize = "large";

                    amountOfRequests = 1;
                }
                else
                {
                    Log.Information($"Something went wrong when generating the file size decider for the high stress test");
                }

                Log.Information($"Amount of requests : {amountOfRequests}");

                 var t1 = _unaryRequestService.UnaryBatchIterativeAsync(null, amountOfRequests, fileSize);

                 var t2 = _streamingLatencyService.CreateManySingleStreamingRequests(null, null, amountOfRequests, fileSize);

                 var t3 = _streamingLatencyService.CreateManyStreamingBatchRequest(null, amountOfRequests, fileSize);

                 var t4 = _unaryRequestService.UnaryResponseIterativeAsync(null, fileSize, amountOfRequests);

                 await Task.WhenAll(t1, t2, t3, t4);  

            }

       
        }

        public async Task MutliClientLowStress(bool lowStressRunning)
        {
            string fileSize = "small";

            

                await _multiClientMultiChannelService.UnaryBatchClientToChannelAllocation(5, fileSize, 1);

                await _multiClientMultiChannelService.StreamingClientToChannelAllocation(5, 1, fileSize);

                await _multiClientMultiChannelService.StreamingBatchClientToChannelAllocation(5, 1, fileSize);

                await _multiClientMultiChannelService.UnaryClientToChannelAllocation(5, fileSize, 1);
            

            Log.Information($"Low stress mutli-client has stopped running");
        }

        public async Task MutliClientMediumStress(bool mediumStressRunning)
        {
           

                string fileSize = "";

                Random random = new Random();

                int number = random.Next(2);

                Log.Information($"Number chose for medium stress : {number}");

                if (number == 0)
                {
                    fileSize = "small";
                }
                else if (number == 1)
                {
                    fileSize = "medium";
                }
                else
                {
                    Log.Information($"Something went wrong with the random number generation for the medium stress");
                }



                await _multiClientMultiChannelService.StreamingClientToChannelAllocation(7, 1, fileSize);

                await _multiClientMultiChannelService.StreamingBatchClientToChannelAllocation(7, 1, fileSize);

                await _multiClientMultiChannelService.UnaryClientToChannelAllocation(7, fileSize, 1);

                await _multiClientMultiChannelService.UnaryBatchClientToChannelAllocation(7, fileSize, 1);

            Log.Information($"Medium stress has stopped running");
        }

        public async Task MultiClientHighStress(bool highStressRunning)
        {
           
                int amountOfRequests = 0;

                string fileSize = "";

                Random random = new Random();

                int number = random.Next(3);

                if (number == 0)
                {
                    fileSize = "small";

                    amountOfRequests = 30;
                }
                else if (number == 1)
                {
                    fileSize = "medium";

                    amountOfRequests = 3;
                }
                else if (number == 2)
                {
                    fileSize = "large";

                    amountOfRequests = 1;
                }
                else
                {
                    Log.Information($"Something went wrong when generating the file size decider for the high stress test");
                }

                Log.Information($"Amount of requests : {amountOfRequests}");

                await _multiClientMultiChannelService.StreamingClientToChannelAllocation(10, 1, fileSize);

                await _multiClientMultiChannelService.StreamingBatchClientToChannelAllocation(10, 1, fileSize);

                await _multiClientMultiChannelService.UnaryClientToChannelAllocation(10, fileSize, 1);

                await _multiClientMultiChannelService.UnaryBatchClientToChannelAllocation(10, fileSize, 1);



            
        }
    }
}
