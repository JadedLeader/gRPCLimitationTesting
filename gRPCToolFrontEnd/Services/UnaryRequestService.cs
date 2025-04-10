﻿using Grpc.Core;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Serilog;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace gRPCToolFrontEnd.Services
{
    public class UnaryRequestService
    {
        private readonly AccountDetailsStore _accountDetailsStore;

        private readonly ClientHelper _clientHelper;

        private readonly ClientInstanceService _clientInstanceService;

        private readonly ClientStorage _clientStorage;

        private readonly SentRequestStorage _sentRequestStorage;

        private readonly GlobalSettings _globalSettings;

        public UnaryRequestService(AccountDetailsStore accountDetailsStore, ClientHelper clientHelper, ClientInstanceService clientInstanceService, ClientStorage clientStorage,
            SentRequestStorage sentRequestStorage, GlobalSettings globalSettings)
        {
            _accountDetailsStore = accountDetailsStore;
            _clientHelper = clientHelper;
            _clientInstanceService = clientInstanceService;
            _clientStorage = clientStorage;
            _sentRequestStorage = sentRequestStorage;
            _globalSettings = globalSettings;
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

            return null;
        }

        public async Task<List<DataResponse>> UnaryResponseIterativeAsync(bool isSingleClient, string fileSize, int amountOfIterations, int amountOfChannels)
        {
            List<DataResponse> responseList = new List<DataResponse>();

            ConcurrentDictionary<Guid, GrpcChannel> channels = new ConcurrentDictionary<Guid, GrpcChannel>();

            string filePath = _clientHelper.FileSize(fileSize);

            string content = File.ReadAllText(filePath);

            string dataContent = _clientHelper.DataContentCalc(fileSize);

            string newGuid = Guid.NewGuid().ToString();

            Metadata metaData = new Metadata
            {
                { "request-type", "Unary" },
                { "active-clients", "0" },
                { "data-iterations", "1" },
            };

            if(!isSingleClient)
            {
               channels = _accountDetailsStore.GetChannels();
            }
            else
            {
                channels = await _clientHelper.GeneratingMutlipleChannels(amountOfChannels, _globalSettings.CurrentLocalHost);
            }

            metaData.Add("open-channels", channels.Count.ToString());

            foreach (var channel in channels)
            {
                    int i = 0;

                    while(i < amountOfIterations)
                    {
                        await _clientHelper.PayloadUsage(fileSize);

                        var now = DateTime.UtcNow;
                        long ticks = now.Ticks;
                        string preciseTime = now.ToString("HH:mm:ss.ffffff");

                        CreateClientInstanceResponse newlyCreatedClientInstance = await _clientInstanceService.CreateClientInstanceAsync();

                        DataRequest newRequest = new DataRequest
                        {
                            ClientUnique = newlyCreatedClientInstance.ClientUnique,
                            ConnectionAlive = false,
                            DataContent = content,
                            RequestType = "Unary",
                            DataSize = fileSize,
                            RequestId = Guid.NewGuid().ToString(),
                            RequestTimestamp = preciseTime,
                            DataContentSize = dataContent

                        };

                        Log.Information($"Sending iterative unary request on channel ID {channel.Key} with unique message ID {newRequest.RequestId}");

                        Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(channel.Value);

                        _clientStorage.IncrementUnaryClients();

                        _sentRequestStorage.IncrementSingleUnaryRequest();
                 
                        var response = await newUnaryClient.UnaryResponseAsync(newRequest, metaData);

                        responseList.Add(response);

                        i++;

                    }
             
            }
            
            return responseList;

        }


        public async Task<BatchDataResponse> UnaryBatchResponseAsync(int batchIterations, Guid channelId, string fileSize)
        {
            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelId);

            Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(getChannel.Value);

            _clientStorage.IncrementUnaryClients();

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

        public async Task<List<BatchDataResponse>> UnaryBatchIterativeAsync(bool isSingleClient, int batchIterations, string fileSize, int amountOfChannels)
        {
            List<BatchDataResponse> responseList = new List<BatchDataResponse>();

            List<BatchDataRequestDetails> dataRequestDetails = await GeneratingBatchOfRequests(batchIterations, fileSize);

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            ConcurrentDictionary<Guid, GrpcChannel> channels = new ConcurrentDictionary<Guid, GrpcChannel>();

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

            if(!isSingleClient)
            {
                channels = _accountDetailsStore.GetChannels();
            }
            else
            {
                channels = await _clientHelper.GeneratingMutlipleChannels(amountOfChannels, _globalSettings.CurrentLocalHost);
            }

            Log.Information($"amount of iterations: {batchIterations}");

            foreach (var channel in channels)
            {
                Unary.UnaryClient newUnaryClient = new Unary.UnaryClient(channel.Value);

                _clientStorage.IncrementUnaryClients();

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

                await _clientHelper.PayloadUsage(fileSize);
                _sentRequestStorage.IncrementBatchUnaryRequest(1);

                Log.Information($"New batch data request has been added to the batch, request is owned by client ID {singleRequest.ClientUnique} with over-arching ID : {singleRequest.BatchRequestId} handles {singleRequest.RequestId}");

                dataRequestDetails.Add(singleRequest);

                i++;
            }

            return dataRequestDetails;
        }

        


    }
}
