using System;
using System.Collections.Generic;
using System.Linq;


namespace THREE
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
            Coefficients = coefficients.ToList();

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

        // evaluate the basis functions
        // shBasis is an Array[ 9 ]
        public static void GetBasisAt(Vector3 normal, float[] shBasis)
        {

            // normal is assumed to be unit length

            var x = normal.X;
            var y = normal.Y;
            var z = normal.Z;

            // band 0
            shBasis[0] = 0.282095f;

            // band 1
            shBasis[1] = 0.488603f * y;
            shBasis[2] = 0.488603f * z;
            shBasis[3] = 0.488603f * x;

            // band 2
            shBasis[4] = 1.092548f * x * y;
            shBasis[5] = 1.092548f * y * z;
            shBasis[6] = 0.315392f * (3 * z * z - 1);
            shBasis[7] = 1.092548f * x * z;
            shBasis[8] = 0.546274f * (x * x - y * y);

        }
    }
}
