using Grpc.Core;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.OutputCaching;
using Serilog;
using Serilog.Sinks.File;
using System.Runtime.InteropServices;
using gRPCToolFrontEnd.DataTypes;
using gRPCToolFrontEnd.DictionaryModel;

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

            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique);

            if(getChannel.Value == null)
            {
                Log.Information($"could not find a grpc channel via that GUID");
            }

            StreamingLatency.StreamingLatencyClient newclient = new StreamingLatency.StreamingLatencyClient(getChannel.Value); 

            await GenerateStreamingRequest(newclient, clientUnique, fileSize);
        }

        
        public async Task CreateManySingleStreamingRequests(Guid? channelUnique, int amountOfRequests, string clientUnique, string fileSize)
        {
            Log.Information($"Creating many single streaming requests detected");

            if (channelUnique == null)
            {
                var channels = _accountDetailsStore.GetChannels();

                foreach (var channel in channels)
                {
                    StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(channel.Value);

                    await GeneratingeManySingleStreamingRequests(streamingClient, amountOfRequests, clientUnique, fileSize);
                }
            }
            else
            {
                var getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique.Value);

                StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(getChannel.Value);

                await GeneratingeManySingleStreamingRequests(streamingClient, amountOfRequests, clientUnique, fileSize);
            }

            
        }

        public async Task CreateSingleStreamingBatchRequest(Guid channelUnique, int requestsInBatch, string clientUnique, string fileSize)
        {
            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique);

            if(getChannel.Value == null)
            {
                Log.Warning($"There is no grpc channel established with the channel guid {channelUnique}");
            }

            StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(getChannel.Value);

            await GeneratingSingularBatchStreamingRequest(streamingClient, requestsInBatch, clientUnique, fileSize);
        }

        public async Task CreateManyStreamingBatchRequest(int requestsInBatch, string clientUnique, string fileSize)
        {
            Dictionary<Guid, GrpcChannel> getChannels = _accountDetailsStore.GetChannels();

            if(getChannels.Count == 0)
            {
                Log.Information($"there are no channels available");
            }

            foreach(var channel in getChannels)
            {
                StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(channel.Value);

                await GeneratingSingularBatchStreamingRequest(streamingClient, requestsInBatch, clientUnique, fileSize);
            }

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

                Log.Information($"This is the single streaming request, client Unique : {streamingRequest.ClientUnique}, Message ID: {streamingRequest.RequestId}");

                await call.RequestStream.WriteAsync(streamingRequest);

                
                i++;
            }

            await call.RequestStream.CompleteAsync();

        }


        private async Task<StreamingSingleLatencyRequest> GenerateStreamingRequest(StreamingLatency.StreamingLatencyClient streamingClient, string clientUnique, string fileSize)
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

            return singleRequest;
        }

        

        private async Task GeneratingSingularBatchStreamingRequest(StreamingLatency.StreamingLatencyClient streamingClient, int requestsInBatch, string clientUnique, 
            string fileSize)
        {
            Metadata metadata = new Metadata();

            metadata.Add("data-iterations", requestsInBatch.ToString());

            var call = streamingClient.StreamingSingleBatchRequest(metadata);

            List<StreamingBatchDetailsRequest> requestsToStream = new List<StreamingBatchDetailsRequest>();

            string filePath = _clientHelper.FileSize(fileSize);

            string dataContent = File.ReadAllText(filePath);

            string dataContentSize = _clientHelper.DataContentCalc(fileSize);

            int i = 0; 

            string batchRequestId = Guid.NewGuid().ToString();  

            while(i < requestsInBatch)
            {
                StreamingBatchDetailsRequest streamingBatchDetails = new StreamingBatchDetailsRequest
                {
                    ClientUnique = clientUnique,
                    BatchRequestId = batchRequestId,
                    MessageId = Guid.NewGuid().ToString(),
                    RequestType = "StreamingBatch",
                    ConnectionAlive = true,
                    DataContent = dataContent,
                    DataContentSize = dataContentSize,
                    DataSize = requestsInBatch.ToString(),
                    RequestTimestamp = DateTime.Now.ToString(),   
                };

                Log.Information($"generated one streaming request with client unique : {streamingBatchDetails.ClientUnique} : batch ID {streamingBatchDetails.BatchRequestId} message ID : {streamingBatchDetails.MessageId}");

                requestsToStream.Add(streamingBatchDetails);

                i++;
            }

            Log.Information($"Amount of requests inside the singular streaming batch request {requestsToStream.Count}");

            StreamingBatchLatencyRequest streamingBatchRequest = new StreamingBatchLatencyRequest
            {
                StreamingBatchDataRequest = { requestsToStream }
            };

            await call.RequestStream.WriteAsync(streamingBatchRequest);

            await call.RequestStream.CompleteAsync();
        }

       
       

    }
}
