using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class Uniform : Dictionary<string, object>,ICloneable
    {

        public Uniform() : base() { }

        public Uniform(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public object Clone()
        {
            return this.DeepCopy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Uniform Copy(Uniform original)
        {
            var destination = new Uniform();

            foreach (var entry in original)
            {
                destination.Add(entry.Key, entry.Value);
            }

            return destination;
        }
        public Uniform Copy()
        {
            var destination = new Uniform();

            foreach (var entry in this)
            {
                destination.Add(entry.Key, entry.Value);
            }

            return destination;
        }
    }
}