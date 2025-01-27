using SharedCommonalities.Abstracts;

namespace gRPCToolFrontEnd.LocalStorage
{
    public class HighStressRequestTimingStorage : ListAbstract
    {

        public List<double> HighStressUnarySingle = new();

        public List<double> HighStressUnaryBatch = new();

        public List<double> HighStressStreamingSingle = new();

        public List<double> HighStressStreamingBatch = new();
        public HighStressRequestTimingStorage()
        {
            
        }

        public override void CopyRequestToStorage(List<double> storageList, List<double> requestList)
        {
            base.CopyRequestToStorage(storageList, requestList);
        }

    }
}
