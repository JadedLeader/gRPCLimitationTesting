using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.Abstracts
{
    public abstract class ListAbstract
    {


        public virtual void CopyRequestToStorage(List<double> storageList, List<double> requestList)
        {
            storageList.AddRange(requestList);
        }

    }
}
