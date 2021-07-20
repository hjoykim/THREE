using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Math;
namespace THREE.Lights
{
    public class PointLight : Light
    {
        public new float Power
        {
            get
            {
                return (float)(this.Intensity * 4* System.Math.PI);
            }
            set
            {
                this.Intensity = (float)(value / (4*System.Math.PI));
            }
        }

        public PointLight(Color color, float? itensity = null, float? distance = null, float? decay = null)
            : base(color, itensity)
        {
            this.Distance = distance != null ? (float)distance : 0;
            this.Decay = decay != null ? (float)decay : 1;

            this.Shadow = new PointLightShadow();

            this.type = "PointLight";
        }
    }
}
