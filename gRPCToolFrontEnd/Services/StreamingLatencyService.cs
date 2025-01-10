using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.OutputCaching;
using Serilog;

namespace gRPCToolFrontEnd.Services
{
    public class StreamingLatencyService
    {

        private readonly AccountDetailsStore _accountDetailsStore;  

        public StreamingLatencyService(AccountDetailsStore accountDetailsStore)
        {
            _accountDetailsStore = accountDetailsStore;
        }

        public async Task SendingSingleUnaryRequestStream(Guid channelUnique, string clientUnique, string dataContent, string dataSize)
        {
            Log.Information($"Sending single request in the stream detected");

            var getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique);

            if(getChannel.Key == null)
            {
                Log.Information($"could not find a grpc channel via that GUID");
            }

            StreamingLatency.StreamingLatencyClient newclient = new StreamingLatency.StreamingLatencyClient(getChannel.Value); 

            await GenerateStreamingRequest(newclient, clientUnique, dataContent, dataSize);
        }

        public async Task SendingMultipleSequentialRequestStream(Guid channelUnique, int amountOfRequests, string clientInstanceUnique, string dataContent, string dataSize)
        {
            Log.Information($"sending multiple sequential requests in the stream detected");

            var getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique);

            StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(getChannel.Value);

            if(getChannel.Key == null)
            {
                Log.Information($"Could not find a grpc channel via that GUID");
            }

            await GeneratingMultipleSingleRequestStream(streamingClient, amountOfRequests, clientInstanceUnique, dataContent, dataSize);
        }

        private async Task GeneratingMultipleSingleRequestStream(StreamingLatency.StreamingLatencyClient streamingClient, int amountOfRequests, string clientInstanceUnique, string dataContent, string dataSize)
        {
            int i = 0;

            var call = streamingClient.StreamingSingleRequest();

            while(i < amountOfRequests)
            {
                var singleRequest = new StreamingSingleLatencyRequest()
                {
                    ClientUnique = clientInstanceUnique,
                    RequestId = Guid.NewGuid().ToString(),
                    DataContent = dataContent,
                    DataSize = dataSize,
                    ConnectionAlive = true,
                    RequestTimestamp = DateTime.Now.ToString(),
                    RequestType = "Streaming"

                }; 

                Log.Information($"Request in the stream generated with client instance unique : {singleRequest.ClientUnique}, contains request ID : {singleRequest.RequestId}");

               await call.RequestStream.WriteAsync(singleRequest);

                i++;
            }

            await call.RequestStream.CompleteAsync();
        }


        private async Task GenerateStreamingRequest(StreamingLatency.StreamingLatencyClient streamingClient, string clientUnique, string dataContent, string dataSize)
        {
            var call = streamingClient.StreamingSingleRequest();

            var singleRequest = new StreamingSingleLatencyRequest
            {
                ClientUnique = clientUnique,
                RequestId = Guid.NewGuid().ToString(),
                ConnectionAlive = true,
                DataContent = dataContent,
                DataSize = dataSize,
                RequestTimestamp = DateTime.Now.ToString(),
                RequestType = "Streaming",
                
            }; 

            Log.Information($"Sending single request for the stream, with client instance ID {singleRequest.ClientUnique} with message request ID: {singleRequest.RequestId}");

            await call.RequestStream.WriteAsync(singleRequest);

            await call.RequestStream.CompleteAsync();
        }


       

    }
}
