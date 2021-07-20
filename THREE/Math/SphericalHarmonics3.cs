using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Math
{
    public class SphericalHarmonics3 : ICloneable
    {
        public List<Vector3> Coefficients = new List<Vector3>();

        public SphericalHarmonics3()
        {
            for (int i = 0; i < 9; i++)
                Coefficients.Add(Vector3.Zero());
        }
        
        protected SphericalHarmonics3(SphericalHarmonics3 other)
        {
            this.Set(other.Coefficients);
        }

        public SphericalHarmonics3 Set(List<Vector3> coefficients)
        {
            for (int i = 0; i < 9; i++)
            {
                this.Coefficients[i] = coefficients[i];
            }

            return this;
        }

        public SphericalHarmonics3 Zero()
        {
            for (int i = 0; i < 9; i++)
                Coefficients.Add(Vector3.Zero());

            return this;
        }

        public SphericalHarmonics3 Add(SphericalHarmonics3 sh)
        {
            for (int i = 0; i < 9; i++)
            {
                this.Coefficients[i] += sh.Coefficients[i];
            }
            return this;
        }
        public SphericalHarmonics3 Scale(float s)
        {
            for (int i = 0; i < 9; i++)
            {
                this.Coefficients[i] *= s;
            }
            return this;
        }

        public SphericalHarmonics3 Lerp(SphericalHarmonics3 sh, float alpha)
        {
            for (int i = 0; i < 9; i++)
            {
                this.Coefficients[i].Lerp(sh.Coefficients[i], alpha);
            }
            return this;
        }


        public bool Equals(SphericalHarmonics3 sh)
        {
            for (int i = 0; i < 9; i++)
            {
                if (!this.Coefficients[i].Equals(sh.Coefficients[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public object Clone()
        {
            return new SphericalHarmonics3(this);
        }
    }
}
