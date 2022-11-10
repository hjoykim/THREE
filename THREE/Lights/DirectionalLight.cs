using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Math;

namespace THREE.Lights
{
    public class DirectionalLight : Light,ICloneable
    {
        public DirectionalLight(Color color, float? intensity = null) : base(color,intensity)
        {
            this.Position.Copy(Object3D.DefaultUp);

            this.UpdateMatrix();

            this.Target = new Object3D();

            this.Shadow = new DirectionalLightShadow();
           
            this.type = "DirectionalLight";
        }
        public DirectionalLight(int color, float? intensity = null) : this(Color.Hex(color), intensity) { }
        protected DirectionalLight(DirectionalLight other) : base(other)
        {
            this.Target = other.Target;

            this.type = "DirectionalLight";

            this.Shadow = (LightShadow)other.Shadow.Clone();
        }

    }
}
