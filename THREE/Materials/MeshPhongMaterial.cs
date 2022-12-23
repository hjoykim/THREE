using System.Collections;

namespace THREE
{
    public class MeshPhongMaterial : Material
    {
        public MeshPhongMaterial(Hashtable parameter=null)
        {
            this.type = "MeshPhongMaterial";
            this.Color = new Color().SetHex(0xffffff);;
            this.Specular = new Color().SetHex(0x111111); ;
            this.Shininess = 30;

            this.LightMap = null;
            this.LightMapIntensity = 1.0f;

            this.AoMap = null;
            this.AoMapIntensity = 1.0f;

            this.Emissive = new Color().SetHex(0x000000);
            this.EmissiveIntensity = 1.0f;
            this.EmissiveMap = null;

            this.BumpMap = null;
            this.BumpScale = 1;

            this.NormalMap = null;
            this.NormalMapType = Constants.TangentSpaceNormalMap;
            this.NormalScale = new Vector2(1, 1);

            this.DisplacementMap = null;
            this.DisplacementScale = 1;
            this.DisplacementBias = 0;

            this.SpecularMap = null;

            this.AlphaMap = null;

            this.EnvMap = null;

            this.Combine = Constants.MultiplyOperation;

            this.Reflectivity = 1;

            this.RefractionRatio = 0.98f;

            this.Wireframe = false;
            this.WireframeLineWidth = 1;
            this.WireframeLineCap = "round";
            this.WireframeLineJoin = "round";

            this.Skinning = false;
            this.MorphTargets = false;
            this.MorphNormals = false;


            if (parameter != null)
                this.SetValues(parameter);
        }

    }
}
