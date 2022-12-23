using System;

namespace THREE
{
    public class Spherical : ICloneable
    {
        public float Radius;
        public float Phi;
        public float Theta;

        public Spherical(float radius=1,float phi=0,float theta=0)
        {
            Radius = radius;
            Phi = phi;
            Theta = theta;
        }
        public Spherical(Spherical source)
        {
            Radius = source.Radius;
            Phi = source.Phi;
            Theta = source.Theta;
        }
        public object Clone()
        {
            return new Spherical(this);
        }

        public Spherical Set(float radius,float phi,float theta)
        {
            Radius = radius;
            Phi = phi;
            Theta = theta;

            return this;
        }
        public Spherical Copy(Spherical source)
        {
            Radius = source.Radius;
            Phi = source.Phi;
            Theta = source.Theta;

            return this;
        }
        // restrict phi to be betwee EPS and PI-EPS
        public Spherical makeSafe()
        {

            float EPS = 0.000001f;
            this.Phi = (float)System.Math.Max(EPS, System.Math.Min(System.Math.PI - EPS, this.Phi));

            return this;

        }
        public Spherical SetFromVector3(Vector3 v)
        {
            return this.setFromCartesianCoords(v.X, v.Y, v.Z);
        }
        public Spherical setFromCartesianCoords(float x, float y, float z )
        {

            this.Radius = (float)System.Math.Sqrt(x * x + y * y + z * z);

            if (this.Radius == 0)
            {

                this.Theta = 0;
                this.Phi = 0;

            }
            else
            {

                this.Theta = (float)System.Math.Atan2(x, z);
                this.Phi = (float)System.Math.Acos(ExtensionMethods.Clamp(y / this.Radius, -1, 1));

            }
            return this;
        }
    }
}
