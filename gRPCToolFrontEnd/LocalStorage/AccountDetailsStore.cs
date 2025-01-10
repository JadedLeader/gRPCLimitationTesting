using Grpc.Net.Client;
using gRPCToolFrontEnd.DictionaryModel;
using gRPCToolFrontEnd.Services;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace gRPCToolFrontEnd.LocalStorage
{
    public class AccountDetailsStore
    {

        public Dictionary<Guid, GrpcChannel> channels = new Dictionary<Guid, GrpcChannel>();

        public Dictionary<Guid, List<Delay>> ClientInstancesWithMessages = new Dictionary<Guid, List<Delay>>(); 

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

        public Dictionary<Guid, List<Delay>> GetClientsWithMessages()
        {

            return ClientInstancesWithMessages; 
        }

        public async Task<int> ClearChannels()
        {
            channels.Clear();

            return channels.Count;
        }

    }
}
