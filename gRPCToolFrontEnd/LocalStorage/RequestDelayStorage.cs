using gRPCToolFrontEnd.DictionaryModel;
using Serilog;
using System.Collections.Concurrent;

namespace gRPCToolFrontEnd.LocalStorage
{
    public class RequestDelayStorage
    {

        public List<double> StreamingBatchDelays = new List<double>(); 

        public List<double> StreamingSingleDelays = new List<double>();

        public List<double> UnarySingleDelays = new List<double>();

        public List<double> UnaryBatchDelays = new List<double>();

        public async Task AddDelayToDelaysList(List<double> nameOfList, double delayDouble)
        {
            nameOfList.Add(delayDouble);

            Log.Information($"added delay {delayDouble} to list with name {nameOfList} ");
            Log.Information($"total number of items in this list: {nameOfList.Count}");
        }

        public async Task<List<double>> ReturnDelaysList(List<double> nameOfList)
        {
            return nameOfList;
        }

    }
}
