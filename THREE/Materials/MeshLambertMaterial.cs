using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Math;
namespace THREE.Materials
{
    public class MeshLambertMaterial : Material
    {
        public MeshLambertMaterial() : base()
        {
            this.type = "MeshLambertMaterial";

            this.Color = THREE.Math.Color.ColorName(ColorKeywords.white);

            this.Opacity = 1;

            this.Emissive = new Color().SetHex(0x000000);

            this.Combine = Constants.MultiplyOperation;

            this.RefractionRatio = 0.98f;

            this.Transparent = false;

            this.WireframeLineWidth = 1;

            this.WireframeLineCap = "round";
            this.WireframeLineJoin = "round";
        }
        protected MeshLambertMaterial(MeshLambertMaterial source) : base()
        {
            Copy(source);
        }
        public override object Clone()
        {
            var material = new MeshLambertMaterial();
            material.Copy(this);
            return material;
        }
        public object Copy(MeshLambertMaterial source)
        {
            base.Copy(source);
            this.type = source.type;

            this.Color = source.Color;

            this.Opacity = source.Opacity;

            this.Emissive = source.Emissive;

            this.Combine = source.Combine;

            this.RefractionRatio = source.RefractionRatio;

            this.Transparent = source.Transparent;

            this.WireframeLineWidth = source.WireframeLineWidth;

            this.WireframeLineCap = source.WireframeLineCap;
            this.WireframeLineJoin = source.WireframeLineJoin;

            return this;
        }
    }
}
