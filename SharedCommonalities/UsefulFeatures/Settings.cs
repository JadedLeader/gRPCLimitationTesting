using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.UsefulFeatures
{
    public static class Settings
    {
        //this class is going to be used for keeping track of important stats
        //this will probably be renamed to something different later down the line

        //represents the number of active clients currently being used in the application
        private static int NumberOfActiveClients { get; set; }

        //represents what request type is being used
        //the idea here is to create a switch box that when off, this gets changed to false, accepting only streaming requests
        private static bool Unary = true;

        //represents the current number of active channels 
        private static int NumberOfActiveChannels { get; set; }

        //this is true by default so the clients will be removed by the client manager worker as they send their messages
        private static bool RemoveClientsFromChannels = true;

        /// <summary>
        /// Changes the state of the RemoveClientsFromChannels to false
        /// </summary>
        public static void RemoveClientStateFalse()
        {
            RemoveClientsFromChannels = false;
        }

        public static void SetNumberOfActiveChannels(int numberOfChannels)
        {
            NumberOfActiveChannels = numberOfChannels;
        }

        /// <summary>
        /// Returns the state of the RemoveClientsFromChannel 
        /// </summary>
        /// <returns></returns>
        public static bool GetRemoveClientState()
        {
            return RemoveClientsFromChannels; 
        }

        /// <summary>
        /// Increments the active clients by 1
        /// </summary>
        /// <returns>the number of active clients</returns>
        public static int IncrementActiveClients()
        {
            NumberOfActiveClients++;

            return NumberOfActiveClients;
        }


        /// <summary>
        /// Decrements the active clients by 1
        /// </summary>
        /// <returns>the number of active clients after the decrementation</returns>
        public static int DecrementActiveClients()
        {
            NumberOfActiveClients--;

            return NumberOfActiveClients;
        }

        /// <summary>
        /// Used to check the unary state
        /// </summary>
        /// <returns>True is the request type is currently unary, false if it's streaming</returns>
        public static bool UnaryRequestState()
        {
            if(Unary)
            {
                return true;
            }

            return false;
               
        }

        public static int IncrementActiveChannels()
        {
            NumberOfActiveChannels++;

            return NumberOfActiveChannels;
        }

        public static int DecrementActiveChannels()
        {
            NumberOfActiveChannels--;

            return NumberOfActiveChannels;
        }

        public static int GetNumberOfActiveChannels()
        {
            return NumberOfActiveChannels;
        }

        public static int GetNumberOfActiveClients()
        {
            return NumberOfActiveClients;
        }

        public static void SetNumberOfActiveClients(int number)
        {
            NumberOfActiveClients = number;
        }



    }
}
