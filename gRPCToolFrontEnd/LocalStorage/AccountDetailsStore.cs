using Grpc.Net.Client;
using gRPCToolFrontEnd.Services;

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

        public Dictionary<Guid, GrpcChannel> GetChannels()
        {
            return channels;
        }

        public async Task<int> ClearChannels()
        {
            channels.Clear();

            return channels.Count;
        }

      

        

        


    }
}
