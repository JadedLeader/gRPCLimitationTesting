using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService.Storage;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Identity.Client;
using Serilog;
using SharedCommonalities.Storage;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;

namespace gRPCStressTestingService.Services
{
    public class UtilitiesService : IUtilitiesService
    {

        private readonly IDelayCalcRepo _delayCalcRepo;
        private readonly ThroughputStorage _throughputStorage;
        private readonly DelayCalcStorage _delayCalcStorage;

        private DateTime _lastFetchedTimeStreamingBatch = DateTime.MinValue;
        private DateTime _lastFetchedTimeStreamingSingle = DateTime.MinValue;
        private DateTime _lastFetchedTimeUnarySingle = DateTime.MinValue;
        private DateTime _lastFetchedTimeUnaryBatch = DateTime.MinValue;

        public UtilitiesService(IDelayCalcRepo delayCalcRepo, ThroughputStorage throughputStorage, DelayCalcStorage delayCalcStorage)
        {
            _delayCalcRepo = delayCalcRepo;
            _throughputStorage = throughputStorage;
            _delayCalcStorage = delayCalcStorage;
        }


        public async Task GetClientsWithMessages(GetClientsWithMessagesRequest request, IServerStreamWriter<GetClientsWithMessagesResponse> responseStream, ServerCallContext context)
        {

           
            while(!context.CancellationToken.IsCancellationRequested)
            {
                string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

                var getNewDelays = await _delayCalcRepo.GetNewDelays(Guid.Parse(sessionUnique));

                if(getNewDelays == null)
                {
                    Log.Information($"No new delays");
                }

                Log.Information($"this is the count on the new delays grabbed {getNewDelays.Count} ");

                if(getNewDelays.Any())
                {
                    foreach(var kvp in getNewDelays)
                    {
                        foreach(var calc in kvp.Value)
                        {
                            var serverResponse = new GetClientsWithMessagesResponse 
                            {
                                ClientUnique = calc.ClientUnique.ToString(),
                                MessageId = calc.messageId.ToString(),
                                RequestType = calc.RequestType,
                                CommunicationType = calc.CommunicationType,
                                DataIterations = calc.DataIterations,
                                Datacontent = calc.DataContent,
                                Delay = calc.Delay.ToString(),

                            };

                            await responseStream.WriteAsync(serverResponse);
                        }
                    }
                }

            }

        }

        public async Task GetstreamingBatchDelays(GetStreamingBatchDelaysRequest request, IServerStreamWriter<GetStreamingBatchDelaysResponse> responseStream, ServerCallContext context)
        {

            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var newItems = _delayCalcStorage.StreamingBatchStorage
                    .Where(item => item.RecordCreation > _lastFetchedTimeStreamingBatch);

                foreach (var newItem in newItems)
                {
                    GatheringDelays gatheringUnaryBatchDelays = new GatheringDelays
                    {
                        Delay = newItem.Delay.ToString(),
                        MessageId = newItem.messageId.ToString(),
                        RequestType = newItem.RequestType,
                        DataContent = newItem.DataContent,
                        ResponseTimestamp = newItem.RecordCreation.ToString(),
                    };

                    GetStreamingBatchDelaysResponse serverResponse = new GetStreamingBatchDelaysResponse
                    {
                        GatheringStreamingBatchDelays = gatheringUnaryBatchDelays,
                    };

                    await responseStream.WriteAsync(serverResponse);
                }

                if (newItems.Count() > 0)
                {
                    _lastFetchedTimeStreamingBatch = newItems.Max(item => item.RecordCreation);
                }

                await Task.Delay(100);
            }
        }

       

        public async Task GetStreamingDelays(GetStreamingDelaysRequest request, IServerStreamWriter<GetStreamingDelaysResponse> responseStream, ServerCallContext context)
        {
            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var newItems = _delayCalcStorage.StreamingSingleStorage
                    .Where(item => item.RecordCreation > _lastFetchedTimeStreamingSingle);

                foreach (var newItem in newItems)
                {
                    GatheringDelays gatheringUnaryBatchDelays = new GatheringDelays
                    {
                        Delay = newItem.Delay.ToString(),
                        MessageId = newItem.messageId.ToString(),
                        RequestType = newItem.RequestType,
                        DataContent = newItem.DataContent,
                        ResponseTimestamp = newItem.RecordCreation.ToString(),
                    };

                    GetStreamingDelaysResponse serverResponse = new GetStreamingDelaysResponse
                    {
                        GatheringStreamingDelays = gatheringUnaryBatchDelays,
                    };

                    await responseStream.WriteAsync(serverResponse);
                }

                if (newItems.Count() > 0)
                {
                    _lastFetchedTimeStreamingSingle = newItems.Max(item => item.RecordCreation);
                }

                await Task.Delay(100);
            }

        }

        public async Task GetUnaryDelays(GetUnaryDelaysRequest request, IServerStreamWriter<GetUnaryDelaysResponse> responseStream, ServerCallContext context)
        {

            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");


            Log.Information($"unary delays called!!!!!!!!!!");

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var newItems = _delayCalcStorage.UnarySingleStorage
                    .Where(item => item.RecordCreation > _lastFetchedTimeUnarySingle);
     
                foreach (var newItem in newItems)
                {

                    GatheringDelays gatheringStreamingBatchDelays = new GatheringDelays
                    {
                        Delay = newItem.Delay.ToString(),
                        MessageId = newItem.messageId.ToString(),
                        RequestType = newItem.RequestType,
                        DataContent = newItem.DataContent,
                        ResponseTimestamp = newItem.RecordCreation.ToString(),
                    };

                    GetUnaryDelaysResponse serverResponse = new GetUnaryDelaysResponse
                    {
                        GatheringUnaryDelays = gatheringStreamingBatchDelays,
                    };

                    Log.Information($"YIPPIE SUCCESS HAS BEEN HAD {gatheringStreamingBatchDelays.MessageId}");

                    await responseStream.WriteAsync(serverResponse);
                }

                if(newItems.Count() > 0)
                {
                    _lastFetchedTimeUnarySingle = newItems.Max(item => item.RecordCreation);
                }

                await Task.Delay(100);

            }
            
        }

        public async Task GetUnaryBatchDelays(GetUnaryBatchDelaysRequest request, IServerStreamWriter<GetUnaryBatchDelaysResponse> responseStream, ServerCallContext context)
        {

            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

            while(!context.CancellationToken.IsCancellationRequested)
            {
                var newItems = _delayCalcStorage.UnaryBatchStorage
                    .Where(item => item.RecordCreation > _lastFetchedTimeUnaryBatch); 

                foreach(var newItem in newItems)
                {
                    GatheringDelays gatheringUnaryBatchDelays = new GatheringDelays
                    {
                        Delay = newItem.Delay.ToString(),
                        MessageId = newItem.messageId.ToString(),
                        RequestType = newItem.RequestType,
                        DataContent = newItem.DataContent,
                        ResponseTimestamp = newItem.RecordCreation.ToString(),
                    };

                    GetUnaryBatchDelaysResponse serverResponse = new GetUnaryBatchDelaysResponse
                    {
                        GatheringUnaryBatchDelays = gatheringUnaryBatchDelays,
                    }; 

                    await responseStream.WriteAsync(serverResponse);
                }

                if (newItems.Count() > 0)
                {
                    _lastFetchedTimeUnaryBatch = newItems.Max(item => item.RecordCreation);
                }

                await Task.Delay(100);
            }

        }

    }
}

