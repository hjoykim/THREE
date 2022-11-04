using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Math;

namespace THREE.Lights
{
    public class LightProbe : Light,ICloneable
    {

        public LightProbe() : base(Color.ColorName(ColorKeywords.white), null)
        {

        }
        public LightProbe(SphericalHarmonics3 sh, int? intensity) : base(Color.ColorName(ColorKeywords.white),intensity)
        {
            if (sh != null) this.sh = sh;
            else sh = new SphericalHarmonics3();
        }

        protected LightProbe(LightProbe other)
        {
            this.sh = (SphericalHarmonics3)other.sh.Clone();
            this.Intensity = other.Intensity;
        }
    }
}
