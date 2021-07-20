namespace THREE.Math
{
    using System;

    public static class TMath
    {
        private static readonly Random random = new Random();

        public const float PI2 = (2 * 3.14159265358979323846f);

        public const float HalfPI = (3.14159265358979323846f / 2.0f);

        public const float SQRT1_2 = (0.7071067811865476f);

        public const float DEG2RAD = (float)System.Math.PI / 180;

        public static float RadToDeg(double rad)
        {
            return (float)(rad * 180.0f / Math.PI);
        }

        public static float DegToRad(double deg)
        {
            return (float)(Math.PI * deg / 180.0f);
        }
        public static float mapLinear(double x, double a1, double a2, double b1, double b2)
        {
            return (float)(b1 + (x - a1) * (b2 - b1) / (a2 - a1));
        }

        public static float CeilPowerOfTwo(float value)
        {
            return (float)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(value) / Math.Log(2)));

        }

        public static float FloorPowerOfTwo(float value)
        {
            return (float)System.Math.Pow(2, System.Math.Floor(System.Math.Log(value) / Math.Log(2)));
        }

        public static bool IsPowerOfTwo(int value ) 
        {
		    return ( value & ( value - 1 ) ) == 0 && value != 0;
	    }
    }
}
