using SharedCommonalities.ReturnModels.ReturnTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.ObjectMapping
{
    public class ObjectCreation
    {


        public UnaryInfo MappingToUnaryInfo(DateTime? timeOfRequest, TimeSpan? delay, string? typeOfData, int? lengthOfData)
        {
            UnaryInfo unaryInfo = new UnaryInfo()
            {
                TimeOfRequest = timeOfRequest,
                Delay = delay,
                TypeOfData = typeOfData,
                LengthOfData = lengthOfData
            }; 

            return unaryInfo;
        }

        public ClientDetails MappingToClientDetails(Guid requestId, int messageLength, bool isActiveClient)
        {
            ClientDetails clientDetails = new ClientDetails()
            {
                RequestId = requestId,
                MessageLength = messageLength,
                IsActiveClient = isActiveClient
            };

            return clientDetails;
        }

    }
}
