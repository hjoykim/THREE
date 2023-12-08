using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace THREE
{
    public class Vector3d : IEquatable<Vector3d>,INotifyPropertyChanged
    {
        public double X;

        public double Y;

        public double Z;

        public double this[char dirchar]
        {
            get
            {
                if (dirchar == 'x')
                    return this.X;
                else if (dirchar == 'y')
                    return this.Y;
                else if (dirchar == 'z')
                    return this.Z;
                else
                    return 0;
            }
            set
            {
                if (dirchar == 'x')
                    this.X = value;
                else if (dirchar == 'y')
                    this.Y = value;
                else if (dirchar == 'z')
                    this.Z = value;
                else
                    return;
            }
        }
        public Vector3d()
        {
            this.X = this.Y = this.Z = 0;           
        }

        public Vector3d(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        public static Vector3d Zero()
        {
            return new Vector3d(0, 0, 0);
        }

        public static Vector3d One()
        {
            return new Vector3d(1, 1, 1);
        }
        public Vector3d Set(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

            return this;
        }

        public Vector3d SetScalar(double scalar)
        {
            this.X = scalar;
            this.Y = scalar;
            this.Z = scalar;

            return this;
        }

        public Vector3d SetX(double x)
        {
            this.X = x;
            return this;
        }

        public Vector3d SetY(double y)
        {
            this.Y = y;

            return this;
        }

        public Vector3d SetZ(double z)
        {
            this.Z = z;

            return this;
        }

        public Vector3d SetComponent(int index, double value)
        {
            switch (index)
            {
                case 0: this.X = value; break;
                case 1: this.Y = value; break;
                case 2: this.Z = value; break;
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
                case 2: return this.Z;
                default: throw new IndexOutOfRangeException(String.Format("Index {0} is out of rangess", index));
            }
        }

        public Vector3d Clone()
        {
            return new Vector3d(this.X, this.Y,this.Z);
        }

        public Vector3d Copy(Vector3d v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;

            return this;
        }
        public Vector3d Add(Vector3d v)
        {
            this.X += v.X;
            this.Y += v.Y;
            this.Z += v.Z;

            return this;
        }
        public Vector3d AddScalar(double s)
        {
            this.X += s;
            this.Y += s;
            this.Z += s;

            return this;
        }

        public Vector3d AddVectors(Vector3d a, Vector3d b)
        {
            this.X = a.X + b.X;
            this.Y = a.Y + b.Y;
            this.Z = a.Z + b.Z;

            return this;
        }

        public Vector3d AddScaledVector(Vector3d v, double s)
        {
            this.X += v.X * s;
            this.Y += v.Y * s;
            this.Z += v.Z * s;

            return this;
        }

        public static Vector3d operator +(Vector3d v, Vector3d w)
        {
            Vector3d r = new Vector3d();
            r.X = v.X + w.X;
            r.Y = v.Y + w.Y;
            r.Z = v.Z + w.Z;

            return r;
        }

        public static Vector3d operator +(Vector3d v, double s)
        {
            Vector3d r = new Vector3d();

            r.X = v.X + s;
            r.Y = v.Y + s;
            r.Z = v.Z + s;

            return r;
        }

        public Vector3d Sub(Vector3d v)
        {
            this.X -= v.X;
            this.Y -= v.Y;
            this.Z -= v.Z;

            return this;
        }

        public Vector3d SubScalar(double s)
        {
            this.X -= s;
            this.Y -= s;
            this.Z -= s;
            return this;
        }

        public Vector3d SubVectors(Vector3d a, Vector3d b)
        {
            this.X = a.X - b.X;
            this.Y = a.Y - b.Y;
            this.Z = a.Z - b.Z;

            return this;
        }

        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            Vector3d r = new Vector3d();

            r.X = a.X - b.X;
            r.Y = a.Y - b.Y;
            r.Z = a.Z - b.Z;
            return r;
        }

        public static Vector3d operator -(Vector3d a, double s)
        {
            Vector3d r = new Vector3d(); ;
            r.X = a.X - s;
            r.Y = a.Y - s;
            r.Z = a.Z - s;

            return r;
        }
        public Vector3d Multiply(Vector3d v)
        {
            this.X *= v.X;
            this.Y *= v.Y;
            this.Z *= v.Z;

            return this;
        }

        public Vector3d MultiplyScalar(double s)
        {
            this.X *= s;
            this.Y *= s;
            this.Z *= s;

            return this;
        }

        public Vector3d MultiplyVectors(Vector3d a,Vector3d b)
        {
            this.X = a.X * b.X;
            this.Y = a.Y * b.Y;
            this.Z = a.Z * b.Z;

            return this;
        }

        public static Vector3d operator *(Vector3d a, Vector3d b)
        {
            Vector3d r = new Vector3d();
            r.X = a.X * b.X;
            r.Y = a.Y * b.Y;
            r.Z = a.Z * b.Z;
            return r;
        }

        public static Vector3d operator *(Vector3d a, double s)
        {
            Vector3d r = new Vector3d();
            r.X = a.X * s;
            r.Y = a.Y * s;
            r.Z = a.Z * s;
            return r;
        }
        public Vector3d ApplyEuler(Eulerd euler)
        {
            Quaterniond quaternion = new Quaterniond();
            return this.ApplyQuaternion(quaternion.SetFromEuler(euler));
        }

        public Vector3d ApplyAxisAngle(Vector3d axis,double angle)
        {
            Quaterniond quaternion = new Quaterniond();
            return this.ApplyQuaternion(quaternion.SetFromAxisAngle(axis, angle));
        }

        public Vector3d ApplyMatrix3(Matrix3d m)
        {
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            var e = m.Elements;

            this.X = e[0] * x + e[3] * y + e[6] * z;
            this.Y = e[1] * x + e[4] * y + e[7] * z;
            this.Z = e[2] * x + e[5] * y + e[8] * z;

            return this;
        }

        public static Vector3d operator *(Matrix3d m, Vector3d a)
        {
            var x = a.X;
            var y = a.Y;
            var z = a.Z;
            var e = m.Elements;

            Vector3d r = new Vector3d();

            r.X = e[0] * x + e[3] * y + e[6] * z;
            r.Y = e[1] * x + e[4] * y + e[7] * z;
            r.Z = e[2] * x + e[5] * y + e[8] * z;

            return r;
        }

        public Vector3d ApplyNormalMatrix(Matrix3d m)
        {
            return this.ApplyMatrix3(m).Normalize();
        }

        public Vector3d ApplyMatrix4(Matrix4d m)
        {
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            var e = m.Elements;

            var w = 1 / (e[3] * x + e[7] * y + e[11] * z + e[15]);

            this.X = (e[0] * x + e[4] * y + e[8] * z + e[12]) * w;
            this.Y = (e[1] * x + e[5] * y + e[9] * z + e[13]) * w;
            this.Z = (e[2] * x + e[6] * y + e[10] * z + e[14]) * w;

            return this;
        }

        public static Vector3d operator *(Matrix4 m, Vector3d v)
        {
            var x = v.X;
            var y = v.Y;
            var z = v.Z;
            var e = m.Elements;

            var w = 1 / (e[3] * x + e[7] * y + e[11] * z + e[15]);

            Vector3d r = new Vector3d();

            r.X = (e[0] * x + e[4] * y + e[8] * z + e[12]) * w;
            r.Y = (e[1] * x + e[5] * y + e[9] * z + e[13]) * w;
            r.Z = (e[2] * x + e[6] * y + e[10] * z + e[14]) * w;

            return r;
        }

        public Vector3d ApplyQuaternion(Quaterniond q)
        {
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            var qx = q.X;
            var qy = q.Y;
            var qz = q.Z;
            var qw = q.W;

            // calculate quat * vector

            var ix = qw * x + qy * z - qz * y;
            var iy = qw * y + qz * x - qx * z;
            var iz = qw * z + qx * y - qy * x;
            var iw = -qx * x - qy * y - qz * z;

            // calculate result * inverse quat

            this.X = ix * qw + iw * -qx + iy * -qz - iz * -qy;
            this.Y = iy * qw + iw * -qy + iz * -qx - ix * -qz;
            this.Z = iz * qw + iw * -qz + ix * -qy - iy * -qx;

            return this;

        }

        public Vector3d Project(Camera camera)
        {
            return this.ApplyMatrix4(camera.MatrixWorldInverse.ToMatrix4d()).ApplyMatrix4(camera.ProjectionMatrix.ToMatrix4d());
        }

        public Vector3d UnProject(Camera camera)
        {
            return this.ApplyMatrix4(camera.ProjectionMatrixInverse.ToMatrix4d()).ApplyMatrix4(camera.MatrixWorld.ToMatrix4d());
        }

        public Vector3d TransformDirection(Matrix4 m)
        {
            // input: THREE.Matrix4 affine matrix
            // vector interpreted as a direction

            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            var e = m.Elements;

            this.X = e[0] * x + e[4] * y + e[8] * z;
            this.Y = e[1] * x + e[5] * y + e[9] * z;
            this.Z = e[2] * x + e[6] * y + e[10] * z;

            return this.Normalize();
        }

        public Vector3d Divide(Vector3d v)
        {
            this.X /= v.X;
            this.Y /= v.Y;
            this.Z /= v.Z;

            return this;
        }

        
        public Vector3d DivideScalar(double s)
        {
            return this.MultiplyScalar(1 / s);
        }

        public static Vector3d operator /(Vector3d a, Vector3d b)
        {
            Vector3d r = new Vector3d();
            r.X = a.X / b.X;
            r.Y = a.Y / b.Y;
            r.Z = a.Z / b.Z;
            return r;
        }

        public static Vector3d operator /(Vector3d a, double s)
        {
            Vector3d r = new Vector3d();
            r = a * (1 / s);

            return r;
        }



        public Vector3d Min(Vector3d v)
        {
            this.X = System.Math.Min(this.X, v.X);
            this.Y = System.Math.Min(this.Y, v.Y);
            this.Z = System.Math.Min(this.Z, v.Z);

            return this;
        }

        public Vector3d Max(Vector3d v)
        {
            this.X = System.Math.Max(this.X, v.X);
            this.Y = System.Math.Max(this.Y, v.Y);
            this.Z = System.Math.Max(this.Z, v.Z);

            return this;
        }

        public Vector3d Clamp(Vector3d min, Vector3d max)
        {
            this.X = System.Math.Max(min.X, System.Math.Min(max.X, this.X));
            this.Y = System.Math.Max(min.Y, System.Math.Min(max.Y, this.Y));
            this.Z = System.Math.Max(min.Z, System.Math.Min(max.Z, this.Z));

            return this;
        }

        public Vector3d ClampScalar(double minVal, double maxVal)
        {
            this.X = System.Math.Max(minVal, System.Math.Min(maxVal, this.X));
            this.Y = System.Math.Max(minVal, System.Math.Min(maxVal, this.Y));
            this.Z = System.Math.Max(minVal, System.Math.Min(maxVal, this.Z));

            return this;
        }

        public bool Equals(Vector3d v)
        {
            return this.X == v.X && this.Y == v.Y && this.Z == v.Z;
        }

        public Vector3d ClampLength(double min, double max)
        {
            var length = this.Length();

            return this.DivideScalar(length != 0 ? length : 1).MultiplyScalar(System.Math.Max(min, System.Math.Min(max, length)));
        }

        public Vector3d Floor()
        {
            this.X = (double)System.Math.Floor(this.X);
            this.Y = (double)System.Math.Floor(this.Y);
            this.Z = (double)System.Math.Floor(this.Z);

            return this;
        }

        public Vector3d Ceil()
        {
            this.X = (double)System.Math.Ceiling(this.X);
            this.Y = (double)System.Math.Ceiling(this.Y);
            this.Z = (double)System.Math.Ceiling(this.Z);

            return this;
        }

        public Vector3d Round()
        {
            this.X = (double)System.Math.Round(this.X);
            this.Y = (double)System.Math.Round(this.Y);
            this.Z = (double)System.Math.Round(this.Z);

            return this;
        }

        public Vector3d RoundToZero()
        {
            this.X = (this.X < 0) ? (double)System.Math.Ceiling(this.X) : (double)System.Math.Floor(this.X);
            this.Y = (this.Y < 0) ? (double)System.Math.Ceiling(this.Y) : (double)System.Math.Floor(this.Y);
            this.Z = (this.Z < 0) ? (double)System.Math.Ceiling(this.Z) : (double)System.Math.Floor(this.Z);

            return this;
        }

        public Vector3d Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;

            return this;
        }

        public static double Dot(Vector3d v1, Vector3d v2)
        {
            return v1.Dot(v2);
        }
        public double Dot(Vector3d v)
        {
            return this.X * v.X + this.Y * v.Y + this.Z*v.Z;
        }
        
        public double LengthSq()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        public double Length()
        {
            return (double)System.Math.Sqrt(this.LengthSq());
        }

        public double ManhattanLength()
        {
            return (double)(System.Math.Abs(this.X) + System.Math.Abs(this.Y)+System.Math.Abs(this.Z));
        }

        public Vector3d Normalize()
        {
            return this.DivideScalar(this.Length() != 0 ? this.Length() : 1);
        }
        
        public Vector3d SetLength(double length)
        {
            return this.Normalize().MultiplyScalar(length);
        }

        public Vector3d Lerp(Vector3d v, double alpha)
        {
            this.X += (v.X - this.X) * alpha;
            this.Y += (v.Y - this.Y) * alpha;
            this.Z += (v.Z - this.Z) * alpha;

            return this;
        }

        public Vector3d LerpVectors(Vector3d v1, Vector3d v2, double alpha)
        {
            return this.SubVectors(v2, v1).MultiplyScalar(alpha).Add(v1);
        }

        public Vector3d Cross(Vector3d v)
        {
            return this.CrossVectors(this, v);
        }

        public Vector3d CrossVectors(Vector3d a, Vector3d b)
        {
            double ax = a.X, ay = a.Y, az = a.Z;
            double bx = b.X, by = b.Y, bz = b.Z;

            this.X = ay * bz - az * by;
            this.Y = az * bx - ax * bz;
            this.Z = ax * by - ay * bx;

            return this;
        }

        public Vector3d ProjectOnVector(Vector3d v)
        {
            var scalar = v.Dot(this) / v.LengthSq();

            return this.Copy(v).MultiplyScalar(scalar);
            // return v*scalar;
        }

        public Vector3d ProjectOnPlane(Vector3d planeNormal)
        {
            Vector3d _vector = Vector3d.Zero();
            _vector.Copy(this).ProjectOnVector(planeNormal);

            return this.Sub(_vector);
        }

        public Vector3d Reflect(Vector3d normal)
        {
            // reflect incident vector off plane orthogonal to normal
            // normal is assumed to have unit length
            Vector3d _vector = Vector3d.Zero();
            return this.Sub(_vector.Copy(normal).MultiplyScalar(2 * this.Dot(normal)));
        }

        public double AngleTo(Vector3d v)
        {
           var denominator = System.Math.Sqrt( this.LengthSq() * v.LengthSq() );

		    if ( denominator == 0 )
            {
                throw new Exception("THREE.Math.Vector3d: AngleTo() can\'t handle zero length vectors.");
            }

		    double theta = (double)(this.Dot( v ) / denominator);

		    // clamp, to handle numerical problems

		    return (double)System.Math.Acos(Clamp(theta,-1,1));
        }

        public double Clamp(double value, double min, double max)
        {
            return (double)System.Math.Max(min, System.Math.Min(max, value));
        }

        public double DistanceTo(Vector3d v)
        {
            return (double)System.Math.Sqrt(this.DistanceToSquared(v));
        }

        public double DistanceToSquared(Vector3d v)
        {
            var dx = this.X - v.X;
            var dy = this.Y - v.Y;
            var dz = this.Z - v.Z;

            return dx * dx + dy * dy + dz * dz;
        }

        public double ManhattanDistanceTo(Vector3d v)
        {
            return (double)(System.Math.Abs(this.X - v.X) + System.Math.Abs(this.Y - v.Y) + System.Math.Abs(this.Z - v.Z));
        }

        public Vector3d SetFromSpherical(Spherical s)
        {
            return this.SetFromSphericalCoords(s.Radius, s.Phi, s.Theta);
        }

        public Vector3d SetFromSphericalCoords(double radius, double phi, double theta)
        {

            var sinPhiRadius = System.Math.Sin(phi) * radius;

            this.X = (double)(sinPhiRadius * System.Math.Sin(theta));
            this.Y = (double)(System.Math.Cos(phi) * radius);
            this.Z = (double)(sinPhiRadius * System.Math.Cos(theta));

            return this;
        }

        public Vector3d SetFromCylindrical(Cylindrical c)
        {
            return this.SetFromSphericalCoords(c.Radius, c.Theta, c.Y);
        }

        public Vector3d SetFromCylindricalCoords(double radius, double theta, double y)
        {
            this.X = (double)(radius * System.Math.Sin(theta));
            this.Y = y;
            this.Z = (double)(radius * System.Math.Cos(theta));

            return this;
        }

        public Vector3d SetFromMatrixPosition(Matrix4 m)
        {
            var e = m.Elements;

            this.X = e[12];
            this.Y = e[13];
            this.Z = e[14];

            return this;
        }

        public Vector3d SetFromMatrixScale(Matrix4d m)
        {
            var sx = this.SetFromMatrixColumn(m, 0).Length();
            var sy = this.SetFromMatrixColumn(m, 1).Length();
            var sz = this.SetFromMatrixColumn(m, 2).Length();

            this.X = sx;
            this.Y = sy;
            this.Z = sz;

            return this;
        }

        public Vector3d SetFromMatrixColumn(Matrix4d m, int index)
        {
            return this.FromArray(m.Elements, index * 4);
        }

        public Vector3d FromArray(double[] array, int? offset = null)
        {
            int index = 0;
            if (offset != null) index = offset.Value;
            int aLen = array.Length - 1;
            this.X = index<=aLen ? array[index] : double.NaN;
            this.Y = index<=aLen? array[index + 1] : double.NaN;
            this.Z = index<=aLen? array[index + 2] : double.NaN;

            return this;
        }

        public double[] ToArray(double[] array=null, int? offset = null)
        {
            int index = 0;
            if (array == null) array = new double[3];
            if (offset != null) index = offset.Value;

            array[index] = this.X;
            array[index + 1] = this.Y;
            array[index + 2] = this.Z;

            return array;
        }

        public Vector3d FromBufferAttribute(BufferAttribute<double> attribute, int index)
        {
            this.X = attribute.getX(index);
            this.Y = attribute.getY(index);
            this.Z = attribute.getZ(index);

            return this;
        }

        public Vector2d ToVector2()
        {
            return new Vector2d(X, Y);
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float)X, (float)Y, (float)Z);
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
