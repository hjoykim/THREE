using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Materials
{
    public class MeshPhysicalMaterial : MeshStandardMaterial
    {
        public float Clearcoat = 0.0f;

        public float ClearcoatRoughness = 0.0f;

        public float Transparency = 0.0f;
        
        public Vector2 ClearcoatNormalScale;
        public MeshPhysicalMaterial() : base()
        {
            this.type = "MeshPhysicalMaterial";
            
            //this.Defines.Add("STANDARD", ""); already inserted from MeshStandardMaterial
            this.Defines.Add("PHYSICAL", "");

            this.Reflectivity = 0.5f;

            this.ClearcoatNormalScale = new Vector2(1, 1);
            
            this.ClearcoatNormalMap = null;

           

        }
    }
}
