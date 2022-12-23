using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace THREE
{
    public class Vector2d : IEquatable<Vector2d>, INotifyPropertyChanged
    {
        public double X;

        public double Y;

       
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

        public Vector2d()
        {
            this.X = this.Y = 0;
        }

        public Vector2d(double x, double y)
        {
            this.X = x;

            this.Y = y;
        }

        public static Vector2d Zero()
        {
             return new Vector2d(0, 0);
        }

        public Vector2d Set(double x, double y)
        {
            this.X = x;

            this.Y = y;

            return this;
        }

        public void SetScalar(double scalar)
        {
            this.X = scalar;

            this.Y = scalar;
        }

        public Vector2d SetX(double x)
        {
            this.X = x;

            return this;
        }

        public Vector2d SetY(double y)
        {
            this.Y = y;

            return this;
        }

        public Vector2d SetComponent(int index, double value)
        {
            switch (index)
            {
                case 0: this.X = value; break;
                case 1: this.Y = value; break;
                default: throw new IndexOutOfRangeException(String.Format("Index {0} is out of rangess", index));
            }
            return this;
        }

        public double GetComponent(int index)
        {
            switch (index)
            {
                case 0: return this.X;
                case 1: return this.Y;
                default: throw new IndexOutOfRangeException(String.Format("Index {0} is out of rangess", index));
            }
        }

        public Vector2d Clone()
        {
            return new Vector2d(this.X, this.Y);
        }

        public Vector2d Copy(Vector2d v)
        {
            this.X = v.X;
            this.Y = v.Y;

            return this;
        }

        public Vector2d Add(Vector2d v)
        {
            this.X += v.X;
            this.Y += v.Y;

            return this;
        }

        public Vector2d AddVectors(Vector2d a, Vector2d b)
        {
            this.X = a.X + b.X;
            this.Y = a.Y + b.Y;

            return this;
        }
        public static Vector2d operator +(Vector2d v, Vector2d w)
        {
            Vector2d r = new Vector2d();
            r.X = v.X + w.X;
            r.Y = v.Y + w.Y;

            return r;
        }

        public Vector2d AddScalar(double s)
        {
            this.X += s;
            this.Y += s;

            return this;
        }
        public Vector2d AddScaledVector(Vector2d v, double s)
        {
            this.X += v.X * s;
            this.Y += v.Y * s;

            return this;
        }
        public static Vector2d operator +(Vector2d v, double s)
        {
            Vector2d r = new Vector2d();

            r.X = v.X + s;
            r.Y = v.Y + s;

            return r;
        }

        public Vector2d Sub(Vector2d v)
        {
            this.X -= v.X;
            this.Y -= v.Y;

            return this;
        }

        public Vector2d SubScalar(double s)
        {
            this.X -= s;
            this.Y -= s;

            return this;
        }

        public Vector2d SubVectors(Vector2d a, Vector2d b)
        {
            this.X = a.X - b.X;
            this.Y = a.Y - b.Y;

            return this;
        }

        public static Vector2d operator -(Vector2d a, Vector2d b)
        {
            Vector2d r = new Vector2d();

            r.X = a.X - b.X;
            r.Y = a.Y - b.Y;

            return r;
        }

        public static Vector2d operator -(Vector2d a, double s)
        {
            Vector2d r = new Vector2d(); ;
            r.X = a.X - s;
            r.Y = a.Y - s;

            return r;
        }
        public Vector2d Multiply(Vector2d v)
        {
            this.X *= v.X;
            this.Y *= v.Y;

            return this;
        }

        public Vector2d MultiplyScalar(double s)
        {
            this.X *= s;
            this.Y *= s;

            return this;
        }

        public static Vector2d operator *(Vector2d a,Vector2d b)
        {
            Vector2d r = new Vector2d();
            r.X = a.X * b.X;
            r.Y = a.Y * b.Y;

            return r;
        }

        public static Vector2d operator *(Vector2d a, double s)
        {
            Vector2d r = new Vector2d();
            r.X = a.X * s;
            r.Y = a.Y * s;

            return r;
        }

        public Vector2d Divide(Vector2d v)
        {
            this.X /= v.X;
            this.Y /= v.Y;

            return this;
        }

        public Vector2d DivideScalar(double s)
        {
            return this.MultiplyScalar(1 / s);
        }

        public static Vector2d operator /(Vector2d a, Vector2d b)
        {
            Vector2d r = new Vector2d();
            r.X = a.X / b.X;
            r.Y = a.Y / b.Y;

            return r;
        }

        public static Vector2d operator /(Vector2d a, double s)
        {
            Vector2d r = new Vector2d();
            r = a * (1 / s);
            
            return r ;
        }

        public Vector2d ApplyMatrix3(Matrix3 m)
        {
            var x = this.X;
            var y = this.Y;
            var e = m.Elements;

            this.X = e[0] * x + e[3] * y + e[6];
            this.Y = e[1] * x + e[4] * y + e[7];

            return this;
        }

        public static Vector2d operator *(Matrix3 m, Vector2d a)
        {
            var x = a.X;
            var y = a.Y;
            var e = m.Elements;

            Vector2d r = new Vector2d();

            r.X = e[0] * x + e[3] * y + e[6];
            r.Y = e[1] * x + e[4] * y + e[7];

            return r;
        }

        public Vector2d Min(Vector2d v)
        {
            this.X = System.Math.Min(this.X, v.X);
            this.Y = System.Math.Min(this.Y, v.Y);

            return this;
        }

        public Vector2d Max(Vector2d v)
        {
            this.X = System.Math.Max(this.X, v.X);
            this.Y = System.Math.Min(this.Y, v.Y);

            return this;
        }

        public Vector2d Clamp(Vector2d min, Vector2d max)
        {
            this.X = System.Math.Max(min.X, System.Math.Min(max.X, this.X));
            this.Y = System.Math.Max(min.Y, System.Math.Min(max.Y, this.Y));

            return this;
        }

        public Vector2d ClampScalar(double minVal, double maxVal)
        {
            this.X = System.Math.Max(minVal, System.Math.Min(maxVal, this.X));
            this.Y = System.Math.Max(minVal, System.Math.Min(maxVal, this.Y));

            return this;
        }
        
        public bool Equals(Vector2d v)
        {
            return this.X == v.X && this.Y == v.Y;
        }

        public Vector2d ClampLength(double min, double max)
        {
            var length = this.Length();

            return this.DivideScalar(length != 0 ? length : 1).MultiplyScalar(System.Math.Max(min, System.Math.Min(max, length)));
        }

        public Vector2d Floor()
        {
            this.X = System.Math.Floor(this.X);
            this.Y = System.Math.Floor(this.Y);

            return this;
        }

        public Vector2d Ceil()
        {
            this.X = System.Math.Ceiling(this.X);
            this.Y = System.Math.Ceiling(this.Y);

            return this;
        }

        public Vector2d Round()
        {
            this.X = System.Math.Round(this.X);
            this.Y = System.Math.Round(this.Y);

            return this;
        }

        public Vector2d RoundToZero()
        {
            this.X = (this.X < 0) ? System.Math.Ceiling(this.X) : System.Math.Floor(this.X);
            this.Y = (this.Y < 0) ? System.Math.Ceiling(this.Y) : System.Math.Floor(this.Y);

            return this;
        }

        public Vector2d Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;

            return this;
        }

        public double Dot(Vector2d v)
        {
            return this.X * v.X + this.Y * v.Y;
        }

        public double Cross(Vector2d v)
        {
            return this.X * v.Y - this.Y * v.X;
        }

        public double LengthSq()
        {
            return this.X*this.X+this.Y*this.Y;
        }

        public double Length()
        {
            return System.Math.Sqrt(this.LengthSq());
        }

        public double ManhattanLength()
        {
            return (System.Math.Abs(this.X) + System.Math.Abs(this.Y));
        }

        public Vector2d Normalize()
        {
            return this.DivideScalar(this.Length() != 0 ? this.Length() : 1);
        }

        public double Angle()
        {
            // computes the angle in radians with respect to the positive x-axis
            var angle = System.Math.Atan2(this.Y, this.X);
            if (angle < 0) angle += 2 * System.Math.PI;

            return angle;
        }

        public double DistanceTo(Vector2d v)
        {
            return System.Math.Sqrt(this.DistanceToSquared(v));
        }

        public double DistanceToSquared(Vector2d v)
        {
            var dx = this.X - v.X;
            var dy = this.Y - v.Y;

            return dx * dx + dy * dy;
        }

        public double ManhattanDistanceTo(Vector2d v)
        {
            return (System.Math.Abs(this.X - v.X) + System.Math.Abs(this.Y - v.Y));
        }

        public Vector2d SetLength(double length)
        {
            return this.Normalize().MultiplyScalar(length);
        }

        public Vector2d Lerp(Vector2d v, double alpha)
        {
            this.X += (v.X - this.X) * alpha;
            this.Y += (v.Y - this.Y) * alpha;

            return this;
        }

        public Vector2d LerpVectors(Vector2d v1, Vector2d v2, double alpha)
        {
            return this.SubVectors(v2, v1).MultiplyScalar(alpha).Add(v1);
        }

        public Vector2d FromArray(double[] array, int? offset=null)
        {
            int index = 0;
            if (offset != null) index = offset.Value;

            this.X = array[index];
            this.Y = array[index+1];

            return this;
        }

        public double[] ToArray(double[] array=null, int? offset = null)
        {
            
            int index = 0;
            if (array == null) array = new double[2];
            if (offset != null) index = offset.Value;

            array[index] = this.X;
            array[index + 1] = this.Y;

            return array;
        }

        public Vector2d FromBufferAttribute(BufferAttribute<double> attribute, int index)
        {
            this.X = attribute.getX(index);
            this.Y = attribute.getY(index);

            return this;
        }

        public Vector2d RotateAround(Vector2d center, double angle)
        {
            var c = System.Math.Cos(angle);
            var s = System.Math.Sin(angle);

            var x = this.X - center.X;
            var y = this.Y - center.Y;

            this.X = x * c - y * s + center.X;
            this.Y = x * s + y * c + center.Y;

            return this;
        }

        public Vector2 ToVector2()
        {
            return new Vector2((float)X, (float)Y);
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
