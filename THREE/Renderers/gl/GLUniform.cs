using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class GLUniform : Dictionary<string, object>,ICloneable
    {
        public string Id { get; set; }

        public int Addr { get; set; }

        public string UniformKind { get; set; }

        public int UniformType { get; set; }

        public List<object> Cache = new List<object>();

        public GLUniform() : base() 
        {
            UniformKind = "GLUniform";
        }

        public GLUniform(string id) : this()
        {
            Id = id;
        }

        public GLUniform(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public object Clone()
        {
            return this.DeepCopy();
        }

        public GLUniform Copy(GLUniform original)
        {
            return original.DeepCopy();
        }
       
    }
}