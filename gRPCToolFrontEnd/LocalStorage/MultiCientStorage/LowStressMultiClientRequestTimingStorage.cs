using SharedCommonalities.Abstracts;

namespace gRPCToolFrontEnd.LocalStorage.MultiCientStorage
{
    public class LowStressMultiClientRequestTimingStorage : ListAbstract
    {

        public List<double> LowStressUnarySingle = new();

        public List<double> LowStressUnaryBatch = new();

        public List<double> LowStressStreamingSingle = new();

        public List<double> LowStressStreamingBatch = new();

        public LowStressMultiClientRequestTimingStorage()
        {
            
        }

        public override void CopyRequestToStorage(List<double> storageList, List<double> requestList)
        {
            base.CopyRequestToStorage(storageList, requestList);
        }

    }
}
