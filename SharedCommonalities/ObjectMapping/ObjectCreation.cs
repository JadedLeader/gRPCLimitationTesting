using SharedCommonalities.ReturnModels.ReturnTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.ObjectMapping
{
    public class ObjectCreation
    {


        public UnaryInfo MappingToUnaryInfo(DateTime? timeOfRequest, TimeSpan? delay, string? typeOfData, int lengthOfData, string? dataContents, string RequestType, 
            string batchRequestId, string? dataContentSize, object? clientInstance, string dataIterations)
        {
            UnaryInfo unaryInfo = new UnaryInfo()
            {
                TimeOfRequest = timeOfRequest,
                Delay = delay,
                TypeOfData = typeOfData,
                LengthOfData = lengthOfData, 
                DataContents = dataContents,
                RequestType = RequestType, 
                BatchRequestId = batchRequestId, 
                DataContentSize = dataContentSize, 
                ClientInstance = clientInstance, 
                DataIterations = dataIterations
    
            }; 

            return unaryInfo;
        }

        public ClientDetails MappingToClientDetails(Guid messageId, int messageLength, bool isActiveClient, string? dataContent, Guid clientUnique, string dataContentsSize)
        {
            ClientDetails clientDetails = new ClientDetails()
            {
                ClientUnique = clientUnique,
                messageId = messageId,
                MessageLength = messageLength,
                IsActiveClient = isActiveClient,
                DataContent = dataContent,
                DataContentSize = dataContentsSize, 
             
            };

            return clientDetails;
        }

    }
}
