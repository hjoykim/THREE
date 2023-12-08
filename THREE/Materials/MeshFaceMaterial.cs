using System.Collections.Generic;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class MeshFaceMaterial : Material
    {
        public List<Material> Materials;

        public MeshFaceMaterial() : base()
        {
            this.type = "MeshFaceMaterial";
        }
        public MeshFaceMaterial(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
