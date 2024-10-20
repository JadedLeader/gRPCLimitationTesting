using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.Abstracts
{
    public abstract class DictionariesAbstract<Key, Value> 
    {

        public virtual void AddToDictionary(Dictionary<Key, Value> dictionaryName, Key dataToAddKey, Value dataToAddValue)
        {
            dictionaryName.Add(dataToAddKey, dataToAddValue);
        }

        public virtual void RemoveFromDictionary(Dictionary<Key, Value> dictionaryName, Key dataKey)
        {
            dictionaryName.Remove(dataKey);
        }

        public virtual Dictionary<Key, Value> ReturnDictionary(Dictionary<Key, Value> dictionaryName)
        {
            if(dictionaryName == null)
            {
                return null;
            }

            return dictionaryName;
        }

    }
}
