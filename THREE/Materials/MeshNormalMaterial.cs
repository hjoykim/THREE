
namespace THREE
{
    public class MeshNormalMaterial : Material
    {
        public MeshNormalMaterial() : base() 
        {
            this.type = "MeshNormalMaterial";

            this.BumpMap = null;
            this.BumpScale = 1;

            this.NormalMap = null;
            this.NormalMapType = Constants.TangentSpaceNormalMap;
            this.NormalScale = new Vector2(1, 1);

            this.DisplacementMap = null;
            this.DisplacementScale = 1;
            this.DisplacementBias = 0;

            this.Wireframe = false;
            this.WireframeLineWidth = 1;

            this.Fog = false;

            this.Skinning = false;
            this.MorphTargets = false;
            this.MorphNormals = false;
        }
    }
}
