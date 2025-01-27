using SharedCommonalities.Abstracts;

namespace gRPCToolFrontEnd.LocalStorage
{

    /// <summary>
    /// This class handles the storage of the different stress levels alongside all of their results
    /// </summary>
    public class LowStressRequestTimingStorage : ListAbstract
    {
        public List<double> LowStressUnarySingle = new();

        public List<double> LowStressUnaryBatch = new();

        public List<double> LowStressStreamingSingle = new();

        public List<double> LowStressStreamingBatch = new();

        public LowStressRequestTimingStorage()
        {
            
        }

        public override void CopyRequestToStorage(List<double> storageList, List<double> requestList)
        {
            base.CopyRequestToStorage(storageList, requestList);
        }



    }
}
