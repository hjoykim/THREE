using System.Collections;

namespace THREE
{
    public class MeshStandardMaterial : Material
    {
      
        public MeshStandardMaterial() : base()
        {
            this.type = "MeshStandardMaterial";

            this.Defines.Add("STANDARD", "");

            Color = new Color().SetHex(0xffffff);
			Roughness = 1.0f;
			Metalness = 0.0f;

			this.Map = null;

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

			this.RoughnessMap = null;

			this.MetalnessMap = null;

			this.AlphaMap = null;

			this.EnvMap = null;
			this.EnvMapIntensity = 1.0f;

			this.RefractionRatio = 0.98f;

			this.Wireframe = false;
			this.WireframeLineWidth = 1;
			this.WireframeLineCap = "round";
			this.WireframeLineJoin = "round";

			this.Skinning = false;
			this.MorphTargets = false;
			this.MorphNormals = false;

		}
		protected MeshStandardMaterial(MeshStandardMaterial source) :base(source)
        {
			this.type = "MeshStandardMaterial";

			this.Defines = (Hashtable)source.Defines.Clone();

			Color = source.Color;

			this.Map = source.Map;//!=null ? (Texture)source.Map.Clone() : null;

			this.LightMap = source.LightMap;//!=null?(Texture)source.LightMap.Clone() : null;
			this.LightMapIntensity = source.LightMapIntensity;

			this.AoMap = source.AoMap;// != null ? (Texture)source.AoMap.Clone() : null;
			this.AoMapIntensity = source.AoMapIntensity;

			this.Emissive = source.Emissive;
			this.EmissiveIntensity = source.EmissiveIntensity;
			this.EmissiveMap = source.EmissiveMap;//!=null ? (Texture)source.EmissiveMap.Clone():null;

			this.BumpMap = source.BumpMap;// != null ? (Texture)source.BumpMap.Clone() : null;
			this.BumpScale = source.BumpScale;

			this.NormalMap = source.NormalMap;// != null ? (Texture)source.NormalMap.Clone() : null; 
			this.NormalMapType = source.NormalMapType;
			this.NormalScale = source.NormalScale;

			this.DisplacementMap = source.DisplacementMap;// != null ? (Texture)source.DisplacementMap.Clone() : null;
			this.DisplacementScale = source.DisplacementScale;
			this.DisplacementBias = source.DisplacementBias;

			this.RoughnessMap = source.RoughnessMap;// != null ? (Texture)source.RoughnessMap.Clone() : null;

			this.MetalnessMap = source.MetalnessMap;// != null ? (Texture)source.MetalnessMap.Clone() : null;

			this.AlphaMap = source.AlphaMap;// != null ? (Texture)source.AlphaMap.Clone() : null;

			this.EnvMap = source.EnvMap;// != null ? (Texture)source.EnvMap.Clone() : null;

			this.EnvMapIntensity = source.EnvMapIntensity;

			this.RefractionRatio = source.RefractionRatio;

			this.Wireframe = source.Wireframe;
			this.WireframeLineWidth = source.WireframeLineWidth;
			this.WireframeLineCap = source.WireframeLineCap;
			this.WireframeLineJoin = source.WireframeLineJoin;

			this.Skinning = source.Skinning;
			this.MorphTargets = source.MorphTargets;
			this.MorphNormals = source.MorphNormals;
		}
		public new object Clone()
        {
			return new MeshStandardMaterial(this);
        }
    }
}
