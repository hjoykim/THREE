using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Math;

namespace THREE.Materials
{
    public class MeshPhysicalMaterial : MeshStandardMaterial
    {

        public float Transparency = 0.0f;
        
        public MeshPhysicalMaterial() : base()
        {
            this.type = "MeshPhysicalMaterial";
            
            //this.Defines.Add("STANDARD", ""); already inserted from MeshStandardMaterial
            this.Defines.Add("PHYSICAL", "");

            this.Clearcoat = 0.0f;
            this.ClearcoatRoughness = 0.0f;

            this.Reflectivity = 0.5f;

            this.ClearcoatNormalScale = new Vector2(1, 1);
            
            this.ClearcoatNormalMap = null;

           

        }
    }
}
