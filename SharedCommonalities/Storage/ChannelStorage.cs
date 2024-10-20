using Grpc.Net.Client;
using SharedCommonalities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.Storage
{
    public class ChannelStorage : DictionariesAbstract<Guid, GrpcChannel>
    {
        public static Dictionary<Guid, GrpcChannel> Channels = new Dictionary<Guid, GrpcChannel>();
        public ChannelStorage()
        {
            
        }

        public override void AddToDictionary(Dictionary<Guid, GrpcChannel> dictionaryName, Guid dataToAddKey, GrpcChannel dataToAddValue)
        {
            base.AddToDictionary(dictionaryName, dataToAddKey, dataToAddValue);
        }

        public override void RemoveFromDictionary(Dictionary<Guid, GrpcChannel> dictionaryName, Guid dataKey)
        {
            base.RemoveFromDictionary(dictionaryName, dataKey);
        }

        public override Dictionary<Guid, GrpcChannel> ReturnDictionary(Dictionary<Guid, GrpcChannel> dictionaryName)
        {
            return base.ReturnDictionary(dictionaryName);
        }

    }
}
