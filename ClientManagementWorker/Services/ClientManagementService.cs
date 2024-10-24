using ClientManagementWorkerService.Interfaces;
using SharedCommonalities.ReturnModels.ReturnTypes;
using SharedCommonalities.Storage;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.UsefulFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementWorkerService.Services
{
    public class ClientManagementService : IClientManagementService
    {

        public Dictionary<string, UnaryInfo> clientRequestTiming = new Dictionary<string, UnaryInfo>();

        public Dictionary<Guid, ClientActivity> clientStorage = new Dictionary<Guid, ClientActivity>();

        /// <summary>
        /// This service is going to check the _clientRequestTiming dictionary and the clientStorage dictionary
        /// </summary>
       

        private void GetClientRequestTimingDict()
        {
           // clientRequestTiming = RequestResponseTimeStorage.ReturnClientRequests();
           throw new NotImplementedException();
        }

        private void GetClientStorageDict()
        {
            //clientStorage = ClientStorage.ReturnDictionary()

            throw new NotImplementedException();
        }

        public async Task thing()
        {
            //ClientStorage.AddToClientsDict(Guid.NewGuid(), new ClientActivity { ActiveConnection = true, Client = ""});
            //RequestResponseTimeStorage.AddRequestToList("ebfwyvfewvf", new UnaryInfo());

            GetClientRequestTimingDict();
            GetClientStorageDict();

            Console.WriteLine($"client request timing dict {clientRequestTiming.Count}");
            Console.WriteLine($"client storage dict {clientStorage.Count}");

        }

        public Dictionary<string, UnaryInfo> ReturnClientRequestDict()
        {
            return clientRequestTiming;
        }

        public Dictionary<Guid, ClientActivity> ReturnClients()
        {
            return clientStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RemovingSentRequestsFromClientPool()
        {
           /* if(!Settings.GetRemoveClientState())
            {
                Console.WriteLine($"Not removing clients at this time");
            }

            //based off the key from the client request dictionary we're going to remove that guid from the client storage dict

            GetClientRequestTimingDict(); 
            GetClientStorageDict();

            var clientRequestDict = ReturnClientRequestDict();

            var clientStorageDict = ReturnClients();

            Console.WriteLine($"LENGTH OF CLIENT REQUEST DICT -> {clientRequestDict.Count}");
            Console.WriteLine($"LENGTH OF CLIENT STORAGE DICT -> {clientStorageDict.Count}");

            foreach (var request in  clientStorageDict)
            {
                if(clientRequestDict.ContainsKey(request.Key.ToString()))
                {
                    Console.WriteLine($"client has sent the message successfully, disconnecting client");

                    KeyValuePair<Guid, ClientActivity> client = FindingRecordInClientStorage(request.Key, clientStorageDict);

                    Console.WriteLine($"checking state -> {client.Key} : {client.Value.IsActiveClient}");
                }

            } */ 

            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the record in the client storage dictionary
        /// Sets the active connection to false
        /// </summary>
        /// <param name="uniqueKey"></param>
        /// <param name="clientStorage"></param>
        /// <returns>The key value pair of the record that was found</returns>
        private KeyValuePair<Guid, ClientActivity> FindingRecordInClientStorage(Guid uniqueKey, Dictionary<Guid, ClientActivity> clientStorage)
        {

           /* Console.WriteLine($"Removing record from the client storage");

            var findingRecord = clientStorage.FirstOrDefault(x => x.Key.ToString() == uniqueKey.ToString());

            findingRecord.Value.IsActiveClient = false;

            return findingRecord; */ 

            throw new NotImplementedException() ;
        }


    }
}
