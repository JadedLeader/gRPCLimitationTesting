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
        /// <summary>
        /// This is a permanent track record of all the what what client holds what messages
        /// Guid allocated for the client id
        /// CLientActivity holds a list of client details that holds information such as the message id, the message length, and if it's active
        /// </summary>
        
        public Dictionary<Guid, ClientActivity> Clients = new Dictionary<Guid, ClientActivity>(); //might re-evaluate the use case of this 

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
