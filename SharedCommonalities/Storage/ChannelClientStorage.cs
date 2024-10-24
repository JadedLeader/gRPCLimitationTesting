using Grpc.Net.Client;
using SharedCommonalities.Abstracts;
using SharedCommonalities.ReturnModels.ChannelInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.TimeStorage
{
    public class ChannelClientStorage : DictionariesAbstract<Guid, ChannelInformation>
    {

        public Dictionary<Guid, ChannelInformation> ChannelWithClientInformation = new Dictionary<Guid, ChannelInformation>();

        public override void AddToDictionary(Dictionary<Guid, ChannelInformation> dictionaryName, Guid dataToAddKey, ChannelInformation dataToAddValue)
        {
            base.AddToDictionary(dictionaryName, dataToAddKey, dataToAddValue);
        }

        public override void RemoveFromDictionary(Dictionary<Guid, ChannelInformation> dictionaryName, Guid dataKey)
        {
            base.RemoveFromDictionary(dictionaryName, dataKey);
        }

        public override Dictionary<Guid, ChannelInformation> ReturnDictionary(Dictionary<Guid, ChannelInformation> dictionaryName)
        {
            return base.ReturnDictionary(dictionaryName);
        }

        /// <summary>
        /// removes a client from a channel based on their identifier 
        /// </summary>
        /// <param name="channelIdentifier"></param>
        /// <param name="clientIdentifier"></param>
        /// <returns>the guid of the client that was deleted</returns>
        public Guid RemoveClientFromChannel(Guid channelIdentifier, Guid clientIdentifier)
        {
            
            if(ChannelWithClientInformation.TryGetValue(channelIdentifier, out ChannelInformation channelInformation))
            {
                var findingClientWithin = channelInformation.ClientInfo.FirstOrDefault(c => c.ClientId == clientIdentifier);

                channelInformation.ClientInfo.Remove(findingClientWithin);
            }

            return clientIdentifier;

            
        }

        
    }
}
