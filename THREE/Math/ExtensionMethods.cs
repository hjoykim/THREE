using System;
using System.Collections.Generic;
using System.Linq;


namespace THREE
{
    public static class ExtensionMethods
    {
        public static readonly Random random = new Random();

        public static Color Random(this Color value)
        {
            return new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        }


        //public static Color MultiplyScalar(this Color c, float s)
        //{
        //    float R = (c.R/255)*s;
        //    float G = (c.G / 255) * s;
        //    float B = (c.B / 255) * s;



        //    var color = System.Windows.Media.Color.FromScRgb(c.A,R, G, B);

        //    c = Color.FromArgb(color.A, color.R, color.G, color.B);

        //    return c;
        //}

        //public static Color FromArray(this Color color, float[] source, int offset = 0)
        //{
        //    System.Windows.Media.Color tempColor = System.Windows.Media.Color.FromScRgb(1, source[offset], source[offset + 1], source[offset + 2]);

        //    Color color1 = Color.FromArgb(tempColor.R, tempColor.G, tempColor.B);

        //    return color1;
        //}

        public static float Clamp(this float val, float min, float max)
        {
            //if (val.CompareTo(min) < 0) return min;
            //else if (val.CompareTo(max) > 0) return max;
            //else return val;

            return (float)System.Math.Max(min, System.Math.Min(max, val));
        }

        public static double Clamp(this double val, double min, double max)
        {
            //if (val.CompareTo(min) < 0) return min;
            //else if (val.CompareTo(max) > 0) return max;
            //else return val;

            return System.Math.Max(min, System.Math.Min(max, val));
        }

        public static float[] ToFloatArray(this double[] d)
        {
            float[] f = new float[d.Length];
            for(int i = 0; i < d.Length; i++)
            {
                f[i] = (float)d[i];
            }

            return f;
        }

        public static double[] ToDoubleArray(this float[] f)
        {
            double[] d = new double[f.Length];
            for (int i = 0; i < f.Length; i++)
            {
                d[i] = (double)f[i];
            }

            return d;
        }
        public static float Lerp(this float x, float y, float t)
        {
            return (1 - t) * x + t * y;
            
        }


        public static List<T> Splice<T>(this List<T> source, int index, int count)
        {
            return source.Skip(index).Take(count).ToList();
        }

        public static List<T> Add<T>(this List<T> source,T x,T y,T z)
        {
            source.Add(x);
            source.Add(y);
            source.Add(z);

            return source;
        }

        public static List<T> Add<T>(this List<T> source, T x, T y, T z,T w)
        {
            source.Add(x);
            source.Add(y);
            source.Add(z);
            source.Add(w);

            return source;
        }

        public static List<T> Add<T>(this List<T> source, T x, T y, T z,T x1,T y1,T z1)
        {
            source.Add(x);
            source.Add(y);
            source.Add(z);
            source.Add(x1);
            source.Add(y1);
            source.Add(z1);

            return source;
        }

        public static List<T> Add<T>(this List<T> source, T x, T y)
        {
            source.Add(x);
            source.Add(y);          

            return source;
        }
        public static List<T> Concat<T>(this List<T> source, List<T> target) 
        {
            source.AddRange(target);

            return source;

        }
        public static void Resize2<T>(this List<T> list, int size)
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)
                {
                    list.Capacity = size;
                }

                list.AddRange(new T[size - count]);
            }
        }
        
    }
}
