using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.ReturnModels.ReturnTypes
{
    public class ClientActivity
    {

        List<ClientDetails> clientActivities = new List<ClientDetails>(); 


        public void AddToClientActivities(ClientDetails details)
        {
            clientActivities.Add(details);
        }

        public void AddBatchToClientActivities(List<ClientDetails> clientDetails)
        {
            foreach(ClientDetails clientsRequests in clientDetails)
            {
                clientActivities.Add(clientsRequests);
            }
        }

        public List<ClientDetails> GetClientActivities()
        {
            return clientActivities;
        }
        

    }
}
