namespace THREE.Math
{
    using System;

    public static class Mat
    {
        private static readonly Random random = new Random();

        public const float PI2 = (2 * 3.14159265358979323846f);

        public const float PI = 3.14159265358979323846f;

        public const float HalfPI = (3.14159265358979323846f / 2.0f);

        public const float SQRT1_2 = (0.7071067811865476f);

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
    }
}
