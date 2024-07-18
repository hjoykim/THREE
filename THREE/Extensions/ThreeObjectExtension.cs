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
            return (T)FastDeepCloner.DeepCloner.Clone(source);

        }
    }
}
