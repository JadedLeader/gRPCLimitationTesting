using Grpc.Net.Client;

namespace gRPCToolFrontEnd.LocalStorage
{
    public class AccountDetailsStore
    {

        public Dictionary<Guid, GrpcChannel> channels = new Dictionary<Guid, GrpcChannel>();

        public AccountDetailsStore()
        {
            
        }

        public KeyValuePair<Guid, GrpcChannel> GetGrpcChannel(Guid channelUnique)
        {
            return channels.FirstOrDefault(x => x.Key == channelUnique);
        }

        


    }
}
