using System.Collections;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class RawShaderMaterial : ShaderMaterial
    {
        public RawShaderMaterial(Hashtable parameters = null)
        {
            this.type = "RawShaderMaterial";

            this.SetValues(parameters);
        }
        public RawShaderMaterial(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
