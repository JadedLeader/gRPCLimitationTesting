using ConfigurationStuff.DbModels;
using ConfigurationStuff.Interfaces.Queries;
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
                var getNewDelays = await _delayCalcRepo.GetNewDelays();

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
    }
}
