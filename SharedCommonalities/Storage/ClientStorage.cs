using SharedCommonalities.Abstracts;
using SharedCommonalities.ReturnModels.ReturnTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.Storage
{
    public static class ClientStorage 
    {

        public static Dictionary<Guid, ClientActivity> Clients = new Dictionary<Guid, ClientActivity>();

        public static void AddToClientsDict(Guid dataToAddKey, ClientActivity dataToAddValue)
        {
            Clients.Add(dataToAddKey, dataToAddValue);
        }

        public static void RemoveFromClientDict(Guid dataKey)
        {
            Clients.Remove(dataKey);
        }

        public static Dictionary<Guid, ClientActivity> ReturnDictionary()
        {
            return Clients;
        }
    }
}
