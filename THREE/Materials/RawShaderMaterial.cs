using System.Collections;

namespace THREE
{
    public class RawShaderMaterial : ShaderMaterial
    {
        public RawShaderMaterial(Hashtable parameters = null)
        {
            this.type = "RawShaderMaterial";

            this.SetValues(parameters);
        }
    }
}
