using Grpc.Core;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Serilog;
using System.Threading.Channels;

namespace gRPCToolFrontEnd.Services
{
    public class UnaryRequestService
    {
        private readonly AccountDetailsStore _accountDetailsStore;
        public UnaryRequestService(AccountDetailsStore accountDetailsStore)
        {
            _accountDetailsStore = accountDetailsStore;
        }


        public async Task<DataResponse> UnaryResponseAsync(DataRequest dataRequest, Guid channelId, string dataContentSize)
        {
            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelId);

            Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(getChannel.Value);

            Metadata metaData = new Metadata();

            metaData.Add("request-type", "Unary");
            metaData.Add("open-channels", 0.ToString());
            metaData.Add("active-clients", 0.ToString());
            metaData.Add("data-iterations", "1");
            //metaData.Add("data-content-size", dataContentSize);

            Log.Information($"Sending single unary request on channel ID : {getChannel.Key} ");

           return await newUnaryClient.UnaryResponseAsync(dataRequest, metaData);
        }

        public async Task<List<DataResponse>> UnaryResponseIterativeAsync(DataRequest dataRequest, string dataContentSize)
        {
            List<DataResponse> responseList = new List<DataResponse>();

            Dictionary<Guid, GrpcChannel> channels = _accountDetailsStore.GetChannels();

            foreach(var channel in channels)
            {
                Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(channel.Value);

                Metadata metaData = new Metadata
                {
                    { "request-type", "Unary" },
                    { "open-channels", channels.Count.ToString() },
                    { "active-clients", "0" },
                    { "data-iterations", "1" },
                };

                string uniqueRequestId = Guid.NewGuid().ToString();
                dataRequest.RequestId = uniqueRequestId;

          
                Log.Information($"Sending iterative unary request on channel ID {channel.Key} with unique message ID {dataRequest.RequestId}");

                var response = await newUnaryClient.UnaryResponseAsync(dataRequest, metaData);

                responseList.Add(response);

            }    

            return responseList;

        }


        public async Task<BatchDataResponse> UnaryBatchResponseAsync(BatchDataRequest batchDataRequest, int batchIterations, Guid channelId)
        {
            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelId);

            Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(getChannel.Value);

            Metadata metaData = new Metadata();

            string batchRequestId = Guid.NewGuid().ToString();

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            metaData.Add("batch-iteration", batchIterations.ToString());
            metaData.Add("batch-request-id", batchRequestId);
            metaData.Add("batch-request-timestamp", preciseTime);
            metaData.Add("request-type", "BatchUnary");
            metaData.Add("batch-request-count", batchDataRequest.BatchDataRequest_.Count.ToString());
            metaData.Add("active-clients", "0");
            
            Log.Information($"Sending batch unary request on channel ID: {getChannel.Key}");

            return await newUnaryClient.BatchUnaryResponseAsync(batchDataRequest, metaData);
        }

        public async Task<List<BatchDataResponse>> UnaryBatchIterativeAsync(BatchDataRequest batchDataRequest, int batchIterations)
        {
            List<BatchDataResponse> responseList = new List<BatchDataResponse>();

            Dictionary<Guid, GrpcChannel> channels = _accountDetailsStore.GetChannels();

            foreach (var channel in channels)
            {
                
                Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(channel.Value);

                Metadata metaData = new Metadata();

                string batchRequestId = Guid.NewGuid().ToString();

                foreach(var message in batchDataRequest.BatchDataRequest_)
                {
                    message.RequestId = Guid.NewGuid().ToString();
                    message.BatchRequestId = batchRequestId;
                }

                var now = DateTime.UtcNow;
                long ticks = now.Ticks;
                string preciseTime = now.ToString("HH:mm:ss.ffffff");

                metaData.Add("batch-iteration", batchIterations.ToString());
                metaData.Add("batch-request-id", batchRequestId);
                metaData.Add("batch-request-timestamp", preciseTime);
                metaData.Add("request-type", "BatchUnary");
                metaData.Add("batch-request-count", batchDataRequest.BatchDataRequest_.Count.ToString());
                metaData.Add("active-clients", "0");

                var id = batchDataRequest.BatchDataRequest_[0];

                string requestIdBatch = id.RequestId;

                Log.Information($"Sending iterative batch unary request on channel ID {channel.Key} request ID: request {requestIdBatch} ");

                var response = await newUnaryClient.BatchUnaryResponseAsync(batchDataRequest, metaData);

                responseList.Add(response);
            }

            return responseList;
        }

    }
}
