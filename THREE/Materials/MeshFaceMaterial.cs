using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Materials
{
    public class MeshFaceMaterial : Material
    {
        public List<Material> Materials;

        public MeshFaceMaterial() : base()
        {
            this.type = "MeshFaceMaterial";
        }
    }
}
