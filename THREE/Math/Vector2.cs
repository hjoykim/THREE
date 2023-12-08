using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace THREE
{
    public class Vector2 : IEquatable<Vector2>, INotifyPropertyChanged
    {
        public float X;

        public float Y;

       
        public int Width 
        {
            get
            {
                return (int)X;
            }
            set 
            {
                X = value;
            }
        }

        public int Height 
        {
            get 
            {
                return (int)Y;
            }
            set
            {
                Y = value;
            }
        }

        public Vector2()
        {
            this.X = this.Y = 0;
        }

        public Vector2(float x, float y)
        {
            this.X = x;

            this.Y = y;
        }

        public static Vector2 Zero()
        {
             return new Vector2(0, 0);
        }

        public Vector2 Set(float x, float y)
        {
            this.X = x;

            this.Y = y;

            return this;
        }

        public void SetScalar(float scalar)
        {
            this.X = scalar;

            this.Y = scalar;
        }

        public Vector2 SetX(float x)
        {
            this.X = x;

            return this;
        }

        public Vector2 SetY(float y)
        {
            this.Y = y;

            return this;
        }

        public Vector2 SetComponent(int index, float value)
        {
            switch (index)
            {
                case 0: this.X = value; break;
                case 1: this.Y = value; break;
                default: throw new IndexOutOfRangeException(String.Format("Index {0} is out of rangess", index));
            }
            return this;
        }

        public float GetComponent(int index)
        {
            switch (index)
            {
                case 0: return this.X;
                case 1: return this.Y;
                default: throw new IndexOutOfRangeException(String.Format("Index {0} is out of rangess", index));
            }
        }

        public Vector2 Clone()
        {
            return new Vector2(this.X, this.Y);
        }

        public Vector2 Copy(Vector2 v)
        {
            this.X = v.X;
            this.Y = v.Y;

            return this;
        }

        public Vector2 Add(Vector2 v)
        {
            this.X += v.X;
            this.Y += v.Y;

            return this;
        }

        public Vector2 AddVectors(Vector2 a, Vector2 b)
        {
            this.X = a.X + b.X;
            this.Y = a.Y + b.Y;

            return this;
        }
        public static Vector2 operator +(Vector2 v, Vector2 w)
        {
            Vector2 r = new Vector2();
            r.X = v.X + w.X;
            r.Y = v.Y + w.Y;

            return r;
        }

        public Vector2 AddScalar(float s)
        {
            this.X += s;
            this.Y += s;

            return this;
        }
        public Vector2 AddScaledVector(Vector2 v, float s)
        {
            this.X += v.X * s;
            this.Y += v.Y * s;

            return this;
        }
        public static Vector2 operator +(Vector2 v, float s)
        {
            Vector2 r = new Vector2();

            r.X = v.X + s;
            r.Y = v.Y + s;

            return r;
        }

        public Vector2 Sub(Vector2 v)
        {
            this.X -= v.X;
            this.Y -= v.Y;

            return this;
        }

        public Vector2 SubScalar(float s)
        {
            this.X -= s;
            this.Y -= s;

            return this;
        }

        public Vector2 SubVectors(Vector2 a, Vector2 b)
        {
            this.X = a.X - b.X;
            this.Y = a.Y - b.Y;

            return this;
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            Vector2 r = new Vector2();

            r.X = a.X - b.X;
            r.Y = a.Y - b.Y;

            return r;
        }

        public static Vector2 operator -(Vector2 a, float s)
        {
            Vector2 r = new Vector2(); ;
            r.X = a.X - s;
            r.Y = a.Y - s;

            return r;
        }
        public Vector2 Multiply(Vector2 v)
        {
            this.X *= v.X;
            this.Y *= v.Y;

            return this;
        }

        public Vector2 MultiplyScalar(float s)
        {
            this.X *= s;
            this.Y *= s;

            return this;
        }

        public static Vector2 operator *(Vector2 a,Vector2 b)
        {
            Vector2 r = new Vector2();
            r.X = a.X * b.X;
            r.Y = a.Y * b.Y;

            return r;
        }

        public static Vector2 operator *(Vector2 a, float s)
        {
            Vector2 r = new Vector2();
            r.X = a.X * s;
            r.Y = a.Y * s;

            return r;
        }

        public Vector2 Divide(Vector2 v)
        {
            this.X /= v.X;
            this.Y /= v.Y;

            return this;
        }

        public Vector2 DivideScalar(float s)
        {
            return this.MultiplyScalar(1 / s);
        }

        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            Vector2 r = new Vector2();
            r.X = a.X / b.X;
            r.Y = a.Y / b.Y;

            return r;
        }

        public static Vector2 operator /(Vector2 a, float s)
        {
            Vector2 r = new Vector2();
            r = a * (1 / s);
            
            return r ;
        }

        public Vector2 ApplyMatrix3(Matrix3 m)
        {
            var x = this.X;
            var y = this.Y;
            var e = m.Elements;

            this.X = e[0] * x + e[3] * y + e[6];
            this.Y = e[1] * x + e[4] * y + e[7];

            return this;
        }

        public static Vector2 operator *(Matrix3 m, Vector2 a)
        {
            var x = a.X;
            var y = a.Y;
            var e = m.Elements;

            Vector2 r = new Vector2();

            r.X = e[0] * x + e[3] * y + e[6];
            r.Y = e[1] * x + e[4] * y + e[7];

            return r;
        }

        public Vector2 Min(Vector2 v)
        {
            this.X = System.Math.Min(this.X, v.X);
            this.Y = System.Math.Min(this.Y, v.Y);

            return this;
        }

        public Vector2 Max(Vector2 v)
        {
            this.X = System.Math.Max(this.X, v.X);
            this.Y = System.Math.Min(this.Y, v.Y);

            return this;
        }

        public Vector2 Clamp(Vector2 min, Vector2 max)
        {
            this.X = System.Math.Max(min.X, System.Math.Min(max.X, this.X));
            this.Y = System.Math.Max(min.Y, System.Math.Min(max.Y, this.Y));

            return this;
        }

        public Vector2 ClampScalar(float minVal, float maxVal)
        {
            this.X = System.Math.Max(minVal, System.Math.Min(maxVal, this.X));
            this.Y = System.Math.Max(minVal, System.Math.Min(maxVal, this.Y));

            return this;
        }
        
        public bool Equals(Vector2 v)
        {
            return this.X == v.X && this.Y == v.Y;
        }

        public Vector2 ClampLength(float min, float max)
        {
            var length = this.Length();

            return this.DivideScalar(length != 0 ? length : 1).MultiplyScalar(System.Math.Max(min, System.Math.Min(max, length)));
        }

        public Vector2 Floor()
        {
            this.X = (float)System.Math.Floor(this.X);
            this.Y = (float)System.Math.Floor(this.Y);

            return this;
        }

        public Vector2 Ceil()
        {
            this.X = (float)System.Math.Ceiling(this.X);
            this.Y = (float)System.Math.Ceiling(this.Y);

            return this;
        }

        public Vector2 Round()
        {
            this.X = (float)System.Math.Round(this.X);
            this.Y = (float)System.Math.Round(this.Y);

            return this;
        }

        public Vector2 RoundToZero()
        {
            this.X = (this.X < 0) ? (float)System.Math.Ceiling(this.X) : (float)System.Math.Floor(this.X);
            this.Y = (this.Y < 0) ? (float)System.Math.Ceiling(this.Y) : (float)System.Math.Floor(this.Y);

            return this;
        }

        public Vector2 Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;

            return this;
        }

        public float Dot(Vector2 v)
        {
            return this.X * v.X + this.Y * v.Y;
        }

        public float Cross(Vector2 v)
        {
            return this.X * v.Y - this.Y * v.X;
        }

        public float LengthSq()
        {
            return this.X*this.X+this.Y*this.Y;
        }

        public float Length()
        {
            return (float)System.Math.Sqrt(this.LengthSq());
        }

        public float ManhattanLength()
        {
            return (float)(System.Math.Abs(this.X) + System.Math.Abs(this.Y));
        }

        public Vector2 Normalize()
        {
            return this.DivideScalar(this.Length() != 0 ? this.Length() : 1);
        }

        public float Angle()
        {
            // computes the angle in radians with respect to the positive x-axis
            var angle = System.Math.Atan2(this.Y, this.X);
            if (angle < 0) angle += 2 * System.Math.PI;

            return (float)angle;
        }

        public float DistanceTo(Vector2 v)
        {
            return (float)System.Math.Sqrt(this.DistanceToSquared(v));
        }

        public float DistanceToSquared(Vector2 v)
        {
            var dx = this.X - v.X;
            var dy = this.Y - v.Y;

            return dx * dx + dy * dy;
        }

        public float ManhattanDistanceTo(Vector2 v)
        {
            return (float)(System.Math.Abs(this.X - v.X) + System.Math.Abs(this.Y - v.Y));
        }

        public Vector2 SetLength(float length)
        {
            return this.Normalize().MultiplyScalar(length);
        }

        public Vector2 Lerp(Vector2 v, float alpha)
        {
            this.X += (v.X - this.X) * alpha;
            this.Y += (v.Y - this.Y) * alpha;

            return this;
        }

        public Vector2 LerpVectors(Vector2 v1, Vector2 v2, float alpha)
        {
            return this.SubVectors(v2, v1).MultiplyScalar(alpha).Add(v1);
        }

        public Vector2 FromArray(float[] array, int? offset=null)
        {
            int index = 0;
            if (offset != null) index = offset.Value;

            this.X = array[index];
            this.Y = array[index+1];

            return this;
        }

        public float[] ToArray(float[] array=null, int? offset = null)
        {
            
            int index = 0;
            if (array == null) array = new float[2];
            if (offset != null) index = offset.Value;

            array[index] = this.X;
            array[index + 1] = this.Y;

            return array;
        }

        public Vector2 FromBufferAttribute(BufferAttribute<float> attribute, int index)
        {
            this.X = attribute.getX(index);
            this.Y = attribute.getY(index);

            return this;
        }

        public Vector2 RotateAround(Vector2 center, float angle)
        {
            var c = (float)System.Math.Cos(angle);
            var s = (float)System.Math.Sin(angle);

            var x = this.X - center.X;
            var y = this.Y - center.Y;

            this.X = x * c - y * s + center.X;
            this.Y = x * s + y * c + center.Y;

            return this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
