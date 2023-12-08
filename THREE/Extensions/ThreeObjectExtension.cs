using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace THREE
{
    [Serializable]
    public static class ThreeObjectExtension
    {
        public static T DeepCopy<T>(this T source) where T : new()
        {
            if(!typeof(T).IsSerializable)
            {
                return source;
            }
            try
            {
                object result = null;
                using(var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, source);
                    ms.Position = 0;    
                    result = (T)formatter.Deserialize(ms);
                    ms.Close();
                }

                return (T)result;
            }
            catch(Exception)
            {
                return new T();
            }
        }
    }
}
