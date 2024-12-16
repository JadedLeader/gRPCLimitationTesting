using SharedCommonalities.Abstracts;
using SharedCommonalities.ReturnModels.ReturnTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.Storage
{
    public class ClientStorage : DictionariesAbstract<Guid, ClientActivity>
    {
        //so we're saying overarching ID = client id and the channel information stores or holds message ids  
        public Dictionary<Guid, ClientActivity> Clients = new Dictionary<Guid, ClientActivity>();

        public ClientStorage()
        {
            
        }

        public override void AddToDictionary(Dictionary<Guid, ClientActivity> dictionaryName, Guid dataToAddKey, ClientActivity dataToAddValue)
        {
            base.AddToDictionary(dictionaryName, dataToAddKey, dataToAddValue);
        }

        public override void RemoveFromDictionary(Dictionary<Guid, ClientActivity> dictionaryName, Guid dataKey)
        {
            base.RemoveFromDictionary(dictionaryName, dataKey);
        }

        public override Dictionary<Guid, ClientActivity> ReturnDictionary(Dictionary<Guid, ClientActivity> dictionaryName)
        {
            return base.ReturnDictionary(dictionaryName);
        }
    }
}
