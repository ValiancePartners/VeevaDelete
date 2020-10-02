// MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling
using System;
using System.Runtime.Serialization;

namespace RestUtility.VeevaVaultXml
{
    static class SerializationHelper
    {
        public static void GetValue<T>(SerializationInfo serializationInfo, string name, out T obj)
        {
            object value = serializationInfo.GetValue(name, typeof(T));
            obj = (T)value;
        }

        public static void AddValue<T>(SerializationInfo serializationInfo, string name, T obj)
        {
            serializationInfo.AddValue(name, obj, typeof(T));
        }
    }
}
// end changes by MP on 10/31/2019