using System;
namespace THREE
{

    public static class MathUtils
    {
        public static readonly Random random = new Random();

        public const float PI2 = (2 * 3.14159265358979323846f);

        public const float HalfPI = (3.14159265358979323846f / 2.0f);

        public const float SQRT1_2 = (0.7071067811865476f);

        public const float DEG2RAD = (float)System.Math.PI / 180;
        public const float RAD2DEG = 180 / (float)System.Math.PI;
        public static Color NextColor()
        {
            return new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        }
        public static float NextFloat(float min, float max)
        {
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }
        public static int NextInt()
        {
            return (int)random.NextDouble();
        }
        public static float NextFloat()
        {
            return (float)random.NextDouble();
        }
        public static float RadToDeg(double rad)
        {
            return (float)(rad * 180.0f /System.Math.PI);
        }

        public static float RandFloat(float low,float high)
        {
            return low + (float)random.NextDouble()*(high-low);
        }

        public static float RandFloatSpread(float range)
        {
            return range * (float)(0.5f - random.NextDouble());
        }
        public static float DegToRad(double deg)
        {
            return (float)(System.Math.PI * deg / 180.0f);
        }
        public static float mapLinear(double x, double a1, double a2, double b1, double b2)
        {
            return (float)(b1 + (x - a1) * (b2 - b1) / (a2 - a1));
        }

        public static float CeilPowerOfTwo(float value)
        {
            return (float)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(value) / System.Math.Log(2)));

        }

        public static float FloorPowerOfTwo(float value)
        {
            return (float)(System.Math.Pow(2, System.Math.Floor(System.Math.Log(value) / System.Math.Log(2))));
        }

        public static bool IsPowerOfTwo(int value ) 
        {
		    return ( value & ( value - 1 ) ) == 0 && value != 0;
	    }

        public static float Lerp(float x,float y,float t)
        {
            return (1 - t) * x + t * y;
        }
        public static float Clamp(float val, float min, float max)
        {
            //if (val.CompareTo(min) < 0) return min;
            //else if (val.CompareTo(max) > 0) return max;
            //else return val;

            return (float)System.Math.Max(min, System.Math.Min(max, val));
        }
    }
}
