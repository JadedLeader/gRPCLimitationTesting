﻿using Grpc.Core;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.OutputCaching;
using Serilog;
using Serilog.Sinks.File;
using System.Runtime.InteropServices;
using gRPCToolFrontEnd.DataTypes;
using gRPCToolFrontEnd.DictionaryModel;
using gRPCToolFrontEnd.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using System.Runtime.CompilerServices;
using MudBlazor;
using System.Collections.Concurrent;

namespace gRPCToolFrontEnd.Services
{
    public class StreamingLatencyService
    {

        private readonly AccountDetailsStore _accountDetailsStore;  
        private readonly ClientHelper _clientHelper;
        private readonly ClientInstanceService _clientInstanceService;
        private readonly ClientStorage _clientStorage;
        private readonly SentRequestStorage _sentRequestStorage;
        private readonly GlobalSettings _globalSettings;
        public StreamingLatencyService(AccountDetailsStore accountDetailsStore, ClientHelper clientHelper, ClientInstanceService clientInstanceService, ClientStorage clientStorage,
            SentRequestStorage sentRequestStorage, GlobalSettings globalSettings)
        {
            _accountDetailsStore = accountDetailsStore;
            _clientHelper = clientHelper;
            _clientInstanceService = clientInstanceService;
            _clientStorage = clientStorage;
            _sentRequestStorage = sentRequestStorage;
            _globalSettings = globalSettings;
        }

       
        public async Task SendingSingleUnaryRequestStream(Guid channelUnique, string fileSize)
        {
            Log.Information($"Sending single request in the stream detected");

            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique);

            if (getChannel.Value == null)
            {
                Log.Warning($"could not find a single channel");
                return;
            }

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            StreamingLatency.StreamingLatencyClient newclient = new StreamingLatency.StreamingLatencyClient(getChannel.Value);

            _clientStorage.IncrementStreamingClients();

            await GenerateStreamingRequest(newclient, newlyCreatedClient.ClientUnique, fileSize);
        }

        
        public async Task CreateManySingleStreamingRequests(CancellationToken? cancellationToken, bool isSingleClient, int amountOfRequests, string fileSize, int amountOfChannels)
        {
            Log.Information($"Creating many single streaming requests detected");

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            cancellationToken?.ThrowIfCancellationRequested();

            ConcurrentDictionary<Guid, GrpcChannel> channels = new ConcurrentDictionary<Guid, GrpcChannel>();

            if(!isSingleClient)
            {
                channels = _accountDetailsStore.GetChannels(); 
            }
            else
            {
                channels = await _clientHelper.GeneratingMutlipleChannels(amountOfChannels, _globalSettings.CurrentLocalHost);
            }
            
            if(channels.Count == 0)
            {
                 Log.Warning($"there are no channels available");
                 return;
            }

            foreach (var channel in channels)
            {
                 StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(channel.Value);

                 _clientStorage.IncrementStreamingClients();

                 await GeneratingManySingleStreamingRequests(streamingClient, amountOfRequests, newlyCreatedClient.ClientUnique, fileSize);
            }
          
        }

        public async Task CreateManyStreamingBatchRequest(bool isSingleClient, int requestsInBatch, string fileSize, int amountOfChannels)
        {

            ConcurrentDictionary<Guid, GrpcChannel> channels = new ConcurrentDictionary<Guid, GrpcChannel>();

            if(!isSingleClient)
            {
                channels = _accountDetailsStore.GetChannels();
            }
            else
            {
                channels = await _clientHelper.GeneratingMutlipleChannels(amountOfChannels, _globalSettings.CurrentLocalHost);
            }
            
            Log.Information($"Channel unique was null for the creating many streaming batch requests, defaulting to many gRPC channels");

                
            if (channels.Count == 0)
            {
                 Log.Warning($"there are no channels available");
                 return;
            }

            foreach (var channel in channels)
            {
                CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

                StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(channel.Value);

                _clientStorage.IncrementStreamingClients();

                await GeneratingSingularBatchStreamingRequest(streamingClient, requestsInBatch, newlyCreatedClient.ClientUnique, fileSize);
            }
        }


        private async Task GeneratingManySingleStreamingRequests(StreamingLatency.StreamingLatencyClient streamingClient, int amountOfRequests, string clientUnique, string fileSize)
        {
            var call = streamingClient.StreamingManySingleRequest();

            string filePath = _clientHelper.FileSize(fileSize);

            string dataContent = File.ReadAllText(filePath);

            string dataContentSize = _clientHelper.DataContentCalc(fileSize);

            Metadata metaData = new Metadata(); 

            int i = 0; 

            while(i < amountOfRequests)
            {
                await _clientHelper.PayloadUsage(fileSize);

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

                _sentRequestStorage.IncrementSingleStreamingRequest();

                Log.Information($"This is the single streaming request, client Unique : {streamingRequest.ClientUnique}, Message ID: {streamingRequest.RequestId}");

                await call.RequestStream.WriteAsync(streamingRequest);

                
                i++;
            }

            await call.RequestStream.CompleteAsync();

        }

        public async Task GeneratingSingularBatchStreamingRequest(StreamingLatency.StreamingLatencyClient streamingClient, int requestsInBatch, string clientUnique, 
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
                await _clientHelper.PayloadUsage(fileSize);

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

                _sentRequestStorage.IncrementBatchStreamingRequest(1);

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

        public async Task CreateSingleStreamingBatchRequest(Guid channelUnique, int requestsInBatch, string fileSize)
        {
            KeyValuePair<Guid, GrpcChannel> getChannel = _accountDetailsStore.GetGrpcChannel(channelUnique);

            if (getChannel.Value == null)
            {
                Log.Warning($"There is no grpc channel established with the channel guid {channelUnique}");
                return;
            }

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(getChannel.Value);

            _clientStorage.IncrementStreamingClients();

            await GeneratingSingularBatchStreamingRequest(streamingClient, requestsInBatch, newlyCreatedClient.ClientUnique, fileSize);
        }

        private async Task<StreamingSingleLatencyRequest> GenerateStreamingRequest(StreamingLatency.StreamingLatencyClient streamingClient, string clientUnique, string fileSize)
        {

            string filePath = _clientHelper.FileSize(fileSize);

            string requestContent = File.ReadAllText(filePath);

            string dataContent = _clientHelper.DataContentCalc(fileSize);

            Metadata metaData = new Metadata();

            var call = streamingClient.StreamingSingleRequest();

            await _clientHelper.PayloadUsage(fileSize);

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




    }
}
