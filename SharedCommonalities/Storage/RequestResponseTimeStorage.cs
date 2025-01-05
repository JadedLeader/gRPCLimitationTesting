using SharedCommonalities.Abstracts;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.ReturnModels.ReturnTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharedCommonalities.TimeStorage
{
    public class RequestResponseTimeStorage : DictionariesAbstract<ClientMessageId, UnaryInfo>
    {
        //stores the client request timing, key of the GUID and string of all the other information

        /// <summary>
        /// Client message ID holds a string for a client id and a string for a message id
        /// Unary info holds information about the request made
        /// </summary>
        public ConcurrentDictionary<ClientMessageId, UnaryInfo> _ClientRequestTiming = new ConcurrentDictionary<ClientMessageId, UnaryInfo>();

        //largely the same as the client
        public ConcurrentDictionary<ClientMessageId, UnaryInfo> _ServerResponseTiming = new ConcurrentDictionary<ClientMessageId, UnaryInfo>();
        
        /// <summary>
        /// This is a volatile list and should only be populated once a delay calculation has been made
        /// Once this calculation is added to the DB this requests should then be removed otherwise you get duplicate entries into the DB
        /// ClientMessageId holds two strings, one for the client id and one for the message id
        /// UnaryInfo holds various information about the requests itself 
        /// </summary>
        public ConcurrentDictionary<ClientMessageId, UnaryInfo> _ActualDelayCalculations = new ConcurrentDictionary<ClientMessageId, UnaryInfo>();

        private readonly object _lock = new object();
        public RequestResponseTimeStorage()
        {
           
        }

        public void AddToConcurrentDictLock(ConcurrentDictionary<ClientMessageId, UnaryInfo> dictionary, ClientMessageId key, UnaryInfo value)
        {
            lock (_lock)
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.TryAdd(key, value);
                }
            }
        }

        public bool RemoveFromConcurrentDictLock(ConcurrentDictionary<ClientMessageId, UnaryInfo> dictionary, ClientMessageId key)
        {
            lock (_lock)
            {
                return dictionary.TryRemove(key, out _);
            }
        }

        public ConcurrentDictionary<ClientMessageId, UnaryInfo> ReturnConcurrentDictLock(ConcurrentDictionary<ClientMessageId, UnaryInfo> dictionary)
        {
            lock (_lock)
            {
                // Return a shallow copy for iteration safety
                return new ConcurrentDictionary<ClientMessageId, UnaryInfo>(dictionary);
            }
        }

        public override void AddToDictionary(Dictionary<ClientMessageId, UnaryInfo> dictionaryName, ClientMessageId dataToAddKey, UnaryInfo dataToAddValue)
        {

            base.AddToDictionary(dictionaryName, dataToAddKey, dataToAddValue);
        }

        public override void RemoveFromDictionary(Dictionary<ClientMessageId, UnaryInfo> dictionaryName,ClientMessageId dataKey)
        {
            base.RemoveFromDictionary(dictionaryName, dataKey);
        }

        public override Dictionary<ClientMessageId, UnaryInfo> ReturnDictionary(Dictionary<ClientMessageId, UnaryInfo> dictionaryName)
        {
            return base.ReturnDictionary(dictionaryName);
        }

        /// <summary>
        /// Takes in a Guid, retrieves that client request based on the key from the "ClientRequestTiming" dictionary
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>A RawTimingValue that will contain the ID of the request, the timestamp and the request type</returns>
        /// <exception cref="Exception"></exception>
        public RawTimingValue GettingClientRequestViaGuid(string guid)
        {


            var clientRequest = _ActualDelayCalculations.FirstOrDefault(entry => entry.Key.ClientId == guid);

            if(clientRequest.Key.ClientId == null || clientRequest.Key.MessageId == null)
            {
                throw new Exception($"Either a client request key or a client request value is null here");
            }

            var clientReturn = new RawTimingValue()
            {
                ClientId = clientRequest.Key.ClientId,
                RequestId = clientRequest.Key.MessageId,
                Timestamp = clientRequest.Value.TimeOfRequest.Value,
                RequestType = "Unary"
            };

            return clientReturn;
        }

        /// <summary>
        /// Takes in a guid, retrieves the client response based on the key from the "SeverResponseTiming" dictionary
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>A RawTimingValue type that stores the request ID, the timestamp and the request type</returns>
        /// <exception cref="Exception"></exception>
        public RawTimingValue GettingServerResponseViaGuid(string guid)
        {
            var serverResponse = _ServerResponseTiming.FirstOrDefault(entry => entry.Key.ClientId == guid);

            if(serverResponse.Key.ClientId == null || serverResponse.Key.MessageId == null || serverResponse.Value.TimeOfRequest.Value == null)
            {
                throw new Exception("Either a server response is missing a client id, message id or time value");
            }

            var serverReturn = new RawTimingValue()
            {
                ClientId= serverResponse.Key.ClientId,
                RequestId = serverResponse.Key.MessageId,
                Timestamp = serverResponse.Value.TimeOfRequest.Value,
                RequestType = "Unary"
            }; 

            return serverReturn; 
        }

       
    }
}
