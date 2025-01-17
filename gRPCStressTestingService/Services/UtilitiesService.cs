using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using Serilog;

namespace gRPCStressTestingService.Services
{
    public class UtilitiesService : IUtilitiesService
    {

        private readonly IDelayCalcRepo _delayCalcRepo;
        public UtilitiesService(IDelayCalcRepo delayCalcRepo)
        {
            _delayCalcRepo = delayCalcRepo;
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

            if(sessionUnique == null)
            {
                Log.Information($"the session unique was null when trying to get the streaming batch delays");
            }

            while(!context.CancellationToken.IsCancellationRequested)
            {

                List<DelayCalc> getStreamingBatchRequests = await _delayCalcRepo.GetStreamingBatchRequests(Guid.Parse(sessionUnique));

                if(getStreamingBatchRequests.Count == 0)
                {
                    Log.Information($"There is currently no streaming batch requests");
                }

                foreach(var delay in getStreamingBatchRequests)
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
    }
}
