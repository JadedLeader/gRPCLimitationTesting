using Grpc.Core;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.OutputCaching;
using Serilog;
using Serilog.Sinks.File;

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

            if(getChannel.Key == null)
            {
                Log.Information($"could not find a grpc channel via that GUID");
            }

            StreamingLatency.StreamingLatencyClient newclient = new StreamingLatency.StreamingLatencyClient(getChannel.Value); 

            await GenerateStreamingRequest(newclient, clientUnique, fileSize);
        }

        public async Task SendingMultipleSequentialRequestStream(Guid channelUnique, int amountOfRequests, string clientInstanceUnique, string dataSize, string fileSize)
        {
            Log.Information($"sending multiple sequential requests in the stream detected");

            var getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique);

            StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(getChannel.Value);

            if(getChannel.Key == null)
            {
                Log.Information($"Could not find a grpc channel via that GUID");
            }

            await GeneratingMultipleSingleRequestStream(streamingClient, amountOfRequests, clientInstanceUnique, dataSize, fileSize);
        }

        private async Task GeneratingMultipleSingleRequestStream(StreamingLatency.StreamingLatencyClient streamingClient, int amountOfRequests, string clientInstanceUnique, string dataSize, string fileSize)
        {
            int i = 0;

            string filePath = await GetFilePath(fileSize);

            string requestContent = await GetRequestContent(filePath);

            string dataContent = await GetDataContent(filePath);

            var call = streamingClient.StreamingSingleRequest();

            while(i < amountOfRequests)
            {
                var singleRequest = new StreamingSingleLatencyRequest()
                {
                    ClientUnique = clientInstanceUnique,
                    RequestId = Guid.NewGuid().ToString(),
                    DataContent = requestContent,
                    DataSize = dataSize,
                    ConnectionAlive = true,
                    RequestTimestamp = DateTime.Now.ToString(),
                    RequestType = "Streaming",
                    DataContentSize = dataContent

                }; 

                Log.Information($"Request in the stream generated with client instance unique : {singleRequest.ClientUnique}, contains request ID : {singleRequest.RequestId}");

               await call.RequestStream.WriteAsync(singleRequest);

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

        private async Task<string> GetFilePath(string fileSize)
        {
            string filePath = _clientHelper.FileSize(fileSize);

            if(filePath == null)
            {
                Log.Warning($"File path was null, could not retrieve file path");
            }

            return filePath;

        }

        private async Task<string> GetRequestContent(string filePath)
        {
            string getFilePath = await GetFilePath(filePath);

            string? requestContent = File.ReadAllText(getFilePath);

            if(requestContent == null)
            {
                Log.Warning($"Request content could not be read");
            }

            return requestContent;
            
        }

        private async Task<string> GetDataContent(string filePath)
        {
            string getFilePath = await GetFilePath(filePath);

            string dataContent = _clientHelper.DataContentCalc(filePath);

            if(dataContent == null)
            {
                Log.Warning($"Data content was null with file path {filePath}");
            }

            return dataContent;
        }


       

    }
}
