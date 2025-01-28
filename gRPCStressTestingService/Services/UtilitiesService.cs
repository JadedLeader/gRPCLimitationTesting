using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using Microsoft.AspNetCore.OutputCaching;
using Serilog;
using SharedCommonalities.Storage;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace gRPCStressTestingService.Services
{
    public class UtilitiesService : IUtilitiesService
    {

        private readonly IDelayCalcRepo _delayCalcRepo;
        private readonly ThroughputStorage _throughputStorage;

        public UtilitiesService(IDelayCalcRepo delayCalcRepo, ThroughputStorage throughputStorage)
        {
            _delayCalcRepo = delayCalcRepo;
            _throughputStorage = throughputStorage;
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

                await Task.Delay(500);
            }

        }

        public async Task GetstreamingBatchDelays(GetStreamingBatchDelaysRequest request, IServerStreamWriter<GetStreamingBatchDelaysResponse> responseStream, ServerCallContext context)
        {

            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

            if (sessionUnique == null)
            {
                Log.Information($"the session unique was null when trying to get the streaming batch delays");
            }

            while (!context.CancellationToken.IsCancellationRequested)
            {

                List<DelayCalc> getStreamingBatchRequests = await _delayCalcRepo.GetStreamingBatchRequests(Guid.Parse(sessionUnique));

                if (getStreamingBatchRequests.Count == 0)
                {
                    Log.Information($"There is currently no streaming batch requests");
                }

                foreach (var delay in getStreamingBatchRequests)
                {

                    GatheringDelays gatheringStreamingBatchDelays = new GatheringDelays()
                    {
                        Delay = delay.Delay.ToString(),
                        MessageId = delay.messageId.ToString(),
                        RequestType = delay.RequestType,
                        DataContent = delay.DataContent,
                        ResponseTimestamp = delay.RecordCreation.ToString(),
                    };

                    GetStreamingBatchDelaysResponse serverResponse = new GetStreamingBatchDelaysResponse()
                    {
                        GatheringStreamingBatchDelays = gatheringStreamingBatchDelays
                    };

                    await responseStream.WriteAsync(serverResponse);


                }

                await Task.Delay(500);

            }
        }

        public async Task GetStreamingDelays(GetStreamingDelaysRequest request, IServerStreamWriter<GetStreamingDelaysResponse> responseStream, ServerCallContext context)
        {
            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

            while (!context.CancellationToken.IsCancellationRequested)
            {
                List<DelayCalc> getStreamingRequests = await _delayCalcRepo.GetStreamingRequests(Guid.Parse(sessionUnique));

                if(getStreamingRequests.Count == 0)
                {
                    Log.Warning($"The attempt to get the streaming requests from the database returned nothing");
                }

                foreach (DelayCalc delay in getStreamingRequests)
                {

                    GatheringDelays gatheringStreamingDelays = new GatheringDelays()
                    {
                        MessageId = delay.messageId.ToString(),
                        RequestType = delay.RequestType,
                        DataContent = delay.DataContent,
                        ResponseTimestamp = delay.RecordCreation.ToString(),
                        Delay = delay.Delay.ToString(),
                    };

                    GetStreamingDelaysResponse serverResponse = new GetStreamingDelaysResponse()
                    {
                        GatheringStreamingDelays = gatheringStreamingDelays
                    };

                    await responseStream.WriteAsync(serverResponse);
                }
            }

        }

        public async Task GetUnaryDelays(GetUnaryDelaysRequest request, IServerStreamWriter<GetUnaryDelaysResponse> responseStream, ServerCallContext context)
        {

            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

            while(!context.CancellationToken.IsCancellationRequested)
            {

                var getUnaryDelays = await _delayCalcRepo.GetUnaryRequests(Guid.Parse(sessionUnique));

                if(getUnaryDelays.Count == 0)
                {
                    Log.Warning($"The attempt to get unary requests from the database returned nothing");
                }

                foreach(var unaryDelays in getUnaryDelays)
                {
                    GatheringDelays gatheringUnaryDelays = new GatheringDelays()
                    {
                        MessageId = unaryDelays.messageId.ToString(),
                        RequestType = unaryDelays.RequestType,
                        DataContent = unaryDelays.DataContent,
                        ResponseTimestamp = unaryDelays.RecordCreation.ToString(),
                        Delay = unaryDelays.Delay.ToString(),
                    };

                    GetUnaryDelaysResponse serverResponse = new GetUnaryDelaysResponse()
                    {
                        GatheringUnaryDelays = gatheringUnaryDelays,
                    };

                    await responseStream.WriteAsync(serverResponse);
                }

            }
            
        }

        public async Task GetUnaryBatchDelays(GetUnaryBatchDelaysRequest request, IServerStreamWriter<GetUnaryBatchDelaysResponse> responseStream, ServerCallContext context)
        {

            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

            while(!context.CancellationToken.IsCancellationRequested)
            {

                List<DelayCalc> getBatchUnaryDelays = await _delayCalcRepo.GetBatchUnaryRequests(Guid.Parse(sessionUnique));

                if (getBatchUnaryDelays.Count == 0)
                {
                    Log.Warning($"The attempt to get unary batch requests from the database returned nothing");
                }

                foreach (DelayCalc batchDelays in getBatchUnaryDelays)
                {
                    GatheringDelays gatheringBatchDelays = new GatheringDelays
                    {
                        MessageId = batchDelays.messageId.ToString(),
                        DataContent = batchDelays.DataContent,
                        Delay = batchDelays.Delay.ToString(),
                        RequestType = batchDelays.RequestType,
                        ResponseTimestamp = batchDelays.RecordCreation.ToString(),
                    };

                    GetUnaryBatchDelaysResponse serverResponse = new GetUnaryBatchDelaysResponse
                    {
                        GatheringUnaryBatchDelays = gatheringBatchDelays,
                    };

                    await responseStream.WriteAsync(serverResponse);
                }

            }

        }

        public async Task GetBestThroughput(GetBestThroughputRequest request, IServerStreamWriter<GetBestThroughputResponse> responseStream, ServerCallContext context)
        {
            while(!context.CancellationToken.IsCancellationRequested)
            {

                ConcurrentBag<int> currentThroughputs = await _throughputStorage.ReturnThroughputBag();

                if(currentThroughputs.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
                }
                else
                {
                    int MaxThroughput = currentThroughputs.Max();

                    Log.Information($"Current largest throughput: {MaxThroughput}");

                    GetBestThroughputResponse serverResponse = new GetBestThroughputResponse
                    {
                        BestThroughput = MaxThroughput,
                    };

                    await responseStream.WriteAsync(serverResponse);
                }

            }
        }

        public async Task GetWorstThroughput(GetWorstThroughputRequest request, IServerStreamWriter<GetWorstThroughputResponse> responseStream, ServerCallContext context)
        {
            while(!context.CancellationToken.IsCancellationRequested)
            {

                ConcurrentBag<int> getThroughputBag = await _throughputStorage.ReturnThroughputBag();

                if(getThroughputBag.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
                }
                else
                {

                    int WorstCurrentThroughput = getThroughputBag.Min();

                    Log.Information($"Current smallest throughput: {WorstCurrentThroughput}");

                    GetWorstThroughputResponse serverResponse = new GetWorstThroughputResponse
                    {
                        WorstThroughput = WorstCurrentThroughput,
                    };

                    await responseStream.WriteAsync(serverResponse);
                }

            }
        }



    }
}

