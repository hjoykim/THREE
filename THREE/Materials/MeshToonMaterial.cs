namespace THREE
{
    public class MeshToonMaterial : Material
    {

        public MeshToonMaterial()
        {

            this.type = "MeshToonMaterial";

            Defines.Add("TOON", "");

            Color = new Color().SetHex(0xffffff);
            Specular = new Color().SetHex(0x111111);
            Shininess = 30;

            Map = null;
            GradientMap = null;

            LightMap = null;
            LightMapIntensity = 1.0f;

            AoMap = null;
            AoMapIntensity = 1.0f;

            Emissive = THREE.Color.Hex(0x000000);
            EmissiveIntensity = 1.0f;
            EmissiveMap = null;

            BumpMap = null;
            BumpScale = 1;

            NormalMap = null;
            NormalMapType = Constants.TangentSpaceNormalMap;
            NormalScale = new Vector2(1, 1);

            DisplacementMap = null;
            DisplacementScale = 1;
            DisplacementBias = 0;

            SpecularMap = null;
            
            AlphaMap = null;

            Wireframe = false;

            WireframeLineWidth = 1;

            WireframeLineCap = "round";

            WireframeLineJoin = "round";

            Skinning = false;

            MorphTargets = false;

            MorphNormals = false;
           
           
        }
    }
}
