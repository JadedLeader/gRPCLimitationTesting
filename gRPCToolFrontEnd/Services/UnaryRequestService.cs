using Grpc.Core;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Serilog;
using System.Threading.Channels;

namespace gRPCToolFrontEnd.Services
{
    public class UnaryRequestService
    {
        private readonly AccountDetailsStore _accountDetailsStore;

        private readonly ClientHelper _clientHelper;

        private readonly ClientInstanceService _clientInstanceService;
        public UnaryRequestService(AccountDetailsStore accountDetailsStore, ClientHelper clientHelper, ClientInstanceService clientInstanceService)
        {
            _accountDetailsStore = accountDetailsStore;
            _clientHelper = clientHelper;
            _clientInstanceService = clientInstanceService;
        }

        public async Task<DataResponse> UnaryResponseAsync(Guid channelId, string fileSize)
        {
            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelId);

            Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(getChannel.Value);

            string filePath = _clientHelper.FileSize(fileSize);

            string content = File.ReadAllText(filePath);

            string dataContent = _clientHelper.DataContentCalc(fileSize);

            string newGuid = Guid.NewGuid().ToString();

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            CreateClientInstanceResponse generatedClientInstance = await _clientInstanceService.CreateClientInstanceAsync();

            DataRequest newDataRequest = new DataRequest
            {
                ClientUnique = generatedClientInstance.ClientUnique,
                ConnectionAlive = false,
                DataContent = content,
                RequestType = "Unary",
                DataSize = fileSize,
                RequestId = newGuid,
                RequestTimestamp = preciseTime,
                DataContentSize = dataContent

            };

            Log.Information($"Client ID: {newDataRequest.ClientUnique} has sent message ID: {newDataRequest.RequestId} at {newDataRequest.RequestTimestamp}");

            Metadata metaData = new Metadata();

            metaData.Add("request-type", "Unary");
            metaData.Add("open-channels", 0.ToString());
            metaData.Add("active-clients", 0.ToString());
            metaData.Add("data-iterations", "1");
           
            Log.Information($"Sending single unary request on channel ID : {getChannel.Key} ");

           return await newUnaryClient.UnaryResponseAsync(newDataRequest, metaData);
        }

        public async Task<List<DataResponse>> UnaryResponseIterativeAsync(Guid? channelUnique, string fileSize)
        {
            List<DataResponse> responseList = new List<DataResponse>();

            CreateClientInstanceResponse newlyCreatedClientInstance = await _clientInstanceService.CreateClientInstanceAsync();

            string filePath = _clientHelper.FileSize(fileSize);

            string content = File.ReadAllText(filePath);

            string dataContent = _clientHelper.DataContentCalc(fileSize);

            string newGuid = Guid.NewGuid().ToString();

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            DataRequest newDataRequest = new DataRequest
            {
                ClientUnique = newlyCreatedClientInstance.ClientUnique,
                ConnectionAlive = false,
                DataContent = content,
                RequestType = "Unary",
                DataSize = fileSize,
                RequestId = newGuid,
                RequestTimestamp = preciseTime,
                DataContentSize = dataContent

            };

            Metadata metaData = new Metadata
            {
                { "request-type", "Unary" },
                { "active-clients", "0" },
                { "data-iterations", "1" },
            };

            if (channelUnique == null)
            {
                Log.Information($"Channel unique was null, generating multiple iterative unary requests for all channels");

                Dictionary<Guid, GrpcChannel> channels = _accountDetailsStore.GetChannels();

                foreach (var channel in channels)
                {
                    Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(channel.Value);

                    metaData.Add("open-channels", channels.Count.ToString());

                    string uniqueRequestId = Guid.NewGuid().ToString();
                    newDataRequest.RequestId = uniqueRequestId;

                    Log.Information($"Sending iterative unary request on channel ID {channel.Key} with unique message ID {newDataRequest.RequestId}");

                    var response = await newUnaryClient.UnaryResponseAsync(newDataRequest, metaData);

                    responseList.Add(response);

                }
            }
            else
            {
                Log.Information($"Channel unique was provided, generating multiple iterative unary requests for channel with ID: {channelUnique}");

                KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique.Value);

                Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(getChannel.Value);

                await newUnaryClient.UnaryResponseAsync(newDataRequest, metaData);
            }

            return responseList;

        }


        public async Task<BatchDataResponse> UnaryBatchResponseAsync(int batchIterations, Guid channelId, string fileSize)
        {
            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelId);

            Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(getChannel.Value);

            Metadata metaData = new Metadata();

            string batchRequestId = Guid.NewGuid().ToString();


            List<BatchDataRequestDetails> dataRequestDetails = await GeneratingBatchOfRequests(batchIterations, fileSize);

            Log.Information($"Amount of requests in the data request list: {dataRequestDetails.Count}");

            BatchDataRequest batchDataRequestUnary = new BatchDataRequest
            {
                BatchDataRequest_ = { dataRequestDetails }
            };

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            metaData.Add("batch-iteration", batchIterations.ToString());
            metaData.Add("batch-request-id", batchRequestId);
            metaData.Add("batch-request-timestamp", preciseTime);
            metaData.Add("request-type", "BatchUnary");
            metaData.Add("batch-request-count", batchDataRequestUnary.BatchDataRequest_.Count.ToString());
            metaData.Add("active-clients", "0");
            
            Log.Information($"Sending batch unary request on channel ID: {getChannel.Key}");

            return await newUnaryClient.BatchUnaryResponseAsync(batchDataRequestUnary, metaData);
        }

        public async Task<List<BatchDataResponse>> UnaryBatchIterativeAsync(Guid? channelUnique, int batchIterations, string fileSize)
        {
            List<BatchDataResponse> responseList = new List<BatchDataResponse>();

            List<BatchDataRequestDetails> dataRequestDetails = await GeneratingBatchOfRequests(batchIterations, fileSize);

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            BatchDataRequest batchDataRequestUnary = new BatchDataRequest
            {
                BatchDataRequest_ = { dataRequestDetails }
            };

            string batchRequestId = Guid.NewGuid().ToString();

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            Metadata metaData = new Metadata();

            metaData.Add("batch-iteration", batchIterations.ToString());
            metaData.Add("batch-request-id", batchRequestId);
            metaData.Add("batch-request-timestamp", preciseTime);
            metaData.Add("request-type", "BatchUnary");
            metaData.Add("batch-request-count", batchDataRequestUnary.BatchDataRequest_.Count.ToString());
            metaData.Add("active-clients", "0");

            if (channelUnique == null)
            {
                Log.Information($"No channel unique was provided, generating many unary batch requests for all gRPC channels");

                Dictionary<Guid, GrpcChannel> channels = _accountDetailsStore.GetChannels();

                Log.Information($"amount of iterations: {batchIterations}");

                foreach (var channel in channels)
                {
                    Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(channel.Value);

                    foreach (var message in batchDataRequestUnary.BatchDataRequest_)
                    {
                        message.RequestId = Guid.NewGuid().ToString();
                        message.BatchRequestId = batchRequestId;
                    }

                    var id = batchDataRequestUnary.BatchDataRequest_[0];

                    string requestIdBatch = id.RequestId;

                    Log.Information($"Sending iterative batch unary request on channel ID {channel.Key} request ID: request {requestIdBatch} ");

                    BatchDataResponse response = await newUnaryClient.BatchUnaryResponseAsync(batchDataRequestUnary, metaData);

                    responseList.Add(response);
                }
            }
            else
            {
                Log.Information($"Channel unique was provided, generating batch iterative request for channel with ID : {channelUnique}");

                KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique.Value);

                Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(getChannel.Value);

                BatchDataResponse serverResponse = await newUnaryClient.BatchUnaryResponseAsync(batchDataRequestUnary, metaData);

                responseList.Add(serverResponse);
            }

           

            

            return responseList;
        }

        /// <summary>
        /// This method is in charge of generating each request that goes into the batch of unary requests 
        /// While loop to generate the requests until we reach the amount of requests required in the batch
        /// </summary>
        /// <param name="requestIterations"> is how many messages you want to be in the batch</param>
        /// <returns>A list of BatchDataRequestDetails populated with as many requests stated in the parameter</returns>
        private async Task<List<BatchDataRequestDetails>> GeneratingBatchOfRequests(int requestIterations, string fileSize)
        {
            List<BatchDataRequestDetails> dataRequestDetails = new List<BatchDataRequestDetails>();

            Log.Information($"Request iterations for the batch : {requestIterations}");

            string filePath = _clientHelper.FileSize(fileSize);

            string content = File.ReadAllText(filePath);

            string dataContent = _clientHelper.DataContentCalc(fileSize);

            string batchRequestId = Guid.NewGuid().ToString();

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            CreateClientInstanceResponse newlyCreatedClientInstance = await _clientInstanceService.CreateClientInstanceAsync();

            int i = 0;

            while (i < requestIterations)
            {
                BatchDataRequestDetails singleRequest = new BatchDataRequestDetails
                {
                    ClientUnique = newlyCreatedClientInstance.ClientUnique,
                    ConnectionAlive = true,
                    DataContent = content,
                    DataSize = fileSize,
                    RequestId = Guid.NewGuid().ToString(),
                    RequestTimestamp = preciseTime,
                    RequestType = "BatchUnary",
                    BatchRequestId = batchRequestId,
                    DataContentSize = dataContent
                };

                Log.Information($"New batch data request has been added to the batch, request is owned by client ID {singleRequest.ClientUnique} with over-arching ID : {singleRequest.BatchRequestId} handles {singleRequest.RequestId}");

                dataRequestDetails.Add(singleRequest);

                i++;
            }

            return dataRequestDetails;
        }

    }
}
