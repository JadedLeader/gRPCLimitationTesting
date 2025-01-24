using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;

namespace gRPCToolFrontEnd.Services
{
    public class AdminService
    {
        private readonly ClientHelper _clientHelper;
        public AdminService(ClientHelper clientHelper)
        {
            _clientHelper = clientHelper;
        }

        public async Task ClearChannelResults(string serverAddress)
        {
            var newChannel = GrpcChannel.ForAddress(serverAddress, new GrpcChannelOptions
            {
                MaxSendMessageSize = 100 * 1024 * 1024,
                MaxReceiveMessageSize = 100 * 1024 * 1024,
            });

            Admin.AdminClient newAdminClient = new Admin.AdminClient(newChannel);

            WipeDelayCalcRequest wipleDelayCalc = new WipeDelayCalcRequest()
            { };

            await newAdminClient.ClearDelayCalcAsync(wipleDelayCalc);
        }

    }
}
