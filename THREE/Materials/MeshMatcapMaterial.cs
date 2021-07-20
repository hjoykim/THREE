using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Textures;

namespace THREE.Materials
{
    public class MeshMatcapMaterial : Material
    {
        public Texture Matcap;

        public MeshMatcapMaterial()
            : base()
        {
            this.type = "MeshMatcapMaterial";
        }
    }
}
