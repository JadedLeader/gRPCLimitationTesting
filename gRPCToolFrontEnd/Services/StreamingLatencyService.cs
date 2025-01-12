using Grpc.Core;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.OutputCaching;
using Serilog;
using Serilog.Sinks.File;
using System.Runtime.InteropServices;

namespace gRPCToolFrontEnd.Services
{
    public class StreamingLatencyService
    {

        private readonly AccountDetailsStore _accountDetailsStore;  
        private readonly ClientHelper _clientHelper;

        public StreamingLatencyService(AccountDetailsStore accountDetailsStore, ClientHelper clientHelper)
        {
            _accountDetailsStore = accountDetailsStore;
            _clientHelper = clientHelper;
        }

        public async Task SendingSingleUnaryRequestStream(Guid channelUnique, string clientUnique, string fileSize)
        {
            Log.Information($"Sending single request in the stream detected");

            var getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique);

            if(getChannel.Value == null)
            {
                Log.Information($"could not find a grpc channel via that GUID");
            }

            StreamingLatency.StreamingLatencyClient newclient = new StreamingLatency.StreamingLatencyClient(getChannel.Value); 

            await GenerateStreamingRequest(newclient, clientUnique, fileSize);
        }

        
        public async Task CreateManySingleStreamingRequests(Guid channelUnique, int amountOfRequests, string clientUnique, string fileSize)
        {
            Log.Information($"Creating many single streaming requests detected");

            var channel = _accountDetailsStore.GetGrpcChannel(channelUnique);

            if (channel.Value == null)
            {
                Log.Warning($"The chosen channel does not exist");
            }

            StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(channel.Value);

            await GeneratingeManySingleStreamingRequests(streamingClient, amountOfRequests, clientUnique, fileSize);
        }


        private async Task GeneratingeManySingleStreamingRequests(StreamingLatency.StreamingLatencyClient streamingClient, int amountOfRequests, string clientUnique, string fileSize)
        {
            var call = streamingClient.StreamingManySingleRequest();

            string filePath = _clientHelper.FileSize(fileSize);

            string dataContent = File.ReadAllText(filePath);

            string dataContentSize = _clientHelper.DataContentCalc(fileSize);

            Metadata metaData = new Metadata(); 

            int i = 0; 

            while(i < amountOfRequests)
            {
                StreamingManySingleLatencyRequest streamingRequest = new StreamingManySingleLatencyRequest()
                {
                    ClientUnique = clientUnique,
                    ConnectionAlive = true,
                    DataContent = dataContent,
                    DataContentSize = dataContentSize,
                    DataSize = amountOfRequests.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    RequestTimestamp = DateTime.Now.ToString(),
                    RequestType = "Streaming"
                };

                await call.RequestStream.WriteAsync(streamingRequest);

                i++;
            }

            await call.RequestStream.CompleteAsync();

        }


        private async Task GenerateStreamingRequest(StreamingLatency.StreamingLatencyClient streamingClient, string clientUnique, string fileSize)
        {

            string filePath = _clientHelper.FileSize(fileSize);

            string requestContent = File.ReadAllText(filePath);

            string dataContent = _clientHelper.DataContentCalc(fileSize);

            Metadata metaData = new Metadata();

            var call = streamingClient.StreamingSingleRequest();

           
            var singleRequest = new StreamingSingleLatencyRequest
            {
                ClientUnique = clientUnique,
                RequestId = Guid.NewGuid().ToString(),
                ConnectionAlive = true,
                DataContent = requestContent,
                DataSize = "1",
                RequestTimestamp = DateTime.Now.ToString(),
                RequestType = "Streaming",
                DataContentSize = dataContent
                
            }; 

            Log.Information($"Sending single request for the stream, with client instance ID {singleRequest.ClientUnique} with message request ID: {singleRequest.RequestId}");

            await call.RequestStream.WriteAsync(singleRequest);

            await call.RequestStream.CompleteAsync();
        }

       
       

    }
}
