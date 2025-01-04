using Grpc.Core;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;
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


        public async Task<DataResponse> UnaryResponseAsync(DataRequest dataRequest, Guid channelId)
        {
            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelId);

            Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(getChannel.Value);

            Metadata metaData = new Metadata();

            metaData.Add("request-type", "Unary");
            metaData.Add("open-channels", 0.ToString());
            metaData.Add("active-clients", 0.ToString());
            metaData.Add("data-iterations", "1");

            Log.Information($"Sending single unary request on channel ID : {getChannel.Key} ");

           return await newUnaryClient.UnaryResponseAsync(dataRequest, metaData);
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

        //we need to create a method that takes a list of grpc channels and then produces a unary batch request

    }
}
