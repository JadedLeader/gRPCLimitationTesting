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
            var getAllDelays = await _delayCalcRepo.GetAllDelays();

            foreach (var kvp in getAllDelays)
            {
                Guid ClientUnique = kvp.Key;
                List<DelayCalc> delayCalcs = kvp.Value;

                foreach (var calc in delayCalcs)
                {
                    GetClientsWithMessagesResponse serverResponse = new GetClientsWithMessagesResponse
                    {
                        ClientUnique = ClientUnique.ToString(),
                        MessageId = calc.messageId.ToString(),
                        RequestType = calc.RequestType,
                        CommunicationType = calc.CommunicationType,
                        DataIterations = calc.DataIterations,
                        Datacontent = calc.DataContent,
                        Delay = calc.Delay.ToString(),

                    };

                    //Log.Information($"Client with message server response {serverResponse.ClientUnique} : {serverResponse.MessageId}");

                    await responseStream.WriteAsync(serverResponse);
                }
            }
        }
    }
}
