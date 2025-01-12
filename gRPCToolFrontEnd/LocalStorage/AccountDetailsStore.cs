using Grpc.Net.Client;
using gRPCToolFrontEnd.DataTypes;
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

        public Dictionary<ChannelAndClientInstance, List<Delay>> ChannelsWithClientsAndMessages = new Dictionary<ChannelAndClientInstance, List<Delay>>();

        public AccountDetailsStore()
        {
            
        }

        public async Task AddToChannelsAndClients(ChannelAndClientInstance channelAndClientKeys, List<Delay> delaysToAdd)
        {
            ChannelsWithClientsAndMessages.Add(channelAndClientKeys, delaysToAdd);
        }

        public async Task<Dictionary<ChannelAndClientInstance, List<Delay>>> GetChannelWithClientsAndMessage()
        {
            return ChannelsWithClientsAndMessages;
        }

        public async Task ClearChannelsWithClientsAndMessages()
        {
            ChannelsWithClientsAndMessages.Clear();
        }

        public async Task<KeyValuePair<ChannelAndClientInstance, List<Delay>>> GetChannelViaChannelGuid(Guid channelGuid)
        {
            KeyValuePair<ChannelAndClientInstance, List<Delay>> channelWithMessages = ChannelsWithClientsAndMessages.FirstOrDefault(x => x.Key.ChannelUnique == channelGuid);

            return channelWithMessages;
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
