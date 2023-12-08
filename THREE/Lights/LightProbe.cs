using System;
using System.Linq;

namespace THREE
{
    public class LightProbe : Light,ICloneable
    {

        public LightProbe() : base(Color.ColorName(ColorKeywords.white), null)
        {
            sh = new SphericalHarmonics3();
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
        public LightProbe Copy(LightProbe source)
        {
            if (source.sh != null && source.sh.Coefficients.Count > 0) this.sh.Coefficients = source.sh.Coefficients.ToList(); ;
            Intensity = source.Intensity;
            return this;
        }
    }
}
