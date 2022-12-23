using System.Collections.Generic;

namespace THREE
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
