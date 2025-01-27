using SharedCommonalities.Abstracts;

namespace gRPCToolFrontEnd.LocalStorage
{
    public class MediumStressRequestTimingStorage : ListAbstract
    {

        public List<double> MediumStressUnarySingle = new();

        public List<double> MediumStressUnaryBatch = new();

        public List<double> MediumStressStreamingSingle = new();

        public List<double> MediumStressStreamingBatch = new();

        public MediumStressRequestTimingStorage()
        {
            
        }

        public override void CopyRequestToStorage(List<double> storageList, List<double> requestList)
        {
            base.CopyRequestToStorage(storageList, requestList);
        }

   
    }
}
