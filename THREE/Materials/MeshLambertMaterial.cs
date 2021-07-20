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
    }
}
