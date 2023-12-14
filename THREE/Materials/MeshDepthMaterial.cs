using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class MeshDepthMaterial : Material
    {
        public MeshDepthMaterial() : base()
        {
            this.type = "MeshDepthMaterial";

            this.Skinning = false;

            this.MorphTargets = false;

            this.AlphaMap = null;

            this.DisplacementMap = null;

            this.DisplacementScale = 1.0f;

            this.DisplacementBias = 0.0f;

            this.Wireframe = false;

            this.WireframeLineWidth = 1;

            this.Fog = false;

        }
        public MeshDepthMaterial(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
