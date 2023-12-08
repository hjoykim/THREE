using System.Collections.Generic;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class Uniforms : Dictionary<string, Uniform>,ICloneable
    {
        public Uniforms() :base() { }

        public Uniforms(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public object Clone()
        {
            return this.DeepCopy();
        }

        public Uniforms Copy(Uniforms original)
        {           
            var destination = new Uniforms();

            foreach (var entry in original)
            {
                destination.Add(entry.Key, entry.Value.Copy());
            }

            return destination;
        }

    }
}
