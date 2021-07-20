using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Materials
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
