using gRPCStressTestingService.DTOs.Models.DataRequestModel;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.DTOs.Mapping
{
    public class Mappings
    {

        public DataRequestModel ConvertingDataRequestToModel(DataRequest dataRequest)
        {
            var dataRequestModel = new DataRequestModel()
            {
                RequestType = "Unary",
                RequestId = Guid.NewGuid().ToString(),
                DataSize = dataRequest.DataSize,
                RequestTimestamp = dataRequest.RequestTimestamp,
            }; 

            return dataRequestModel;
        }


    }
}
