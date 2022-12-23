using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace THREE
{
    public class Vector3 : IEquatable<Vector3>,INotifyPropertyChanged
    {
        public float X;

        public float Y;

        public float Z;

        public float this[char dirchar]
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
        public Vector3()
        {
            this.X = this.Y = this.Z = 0;           
        }

        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        public static Vector3 Zero()
        {
            return new Vector3(0, 0, 0);
        }

        public static Vector3 One()
        {
            return new Vector3(1, 1, 1);
        }
        public Vector3 Set(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

            return this;
        }

        public Vector3 SetScalar(float scalar)
        {
            this.X = scalar;
            this.Y = scalar;
            this.Z = scalar;

            return this;
        }

        public Vector3 SetX(float x)
        {
            this.X = x;
            return this;
        }

        public Vector3 SetY(float y)
        {
            this.Y = y;

            return this;
        }

        public Vector3 SetZ(float z)
        {
            this.Z = z;

            return this;
        }

        public Vector3 SetComponent(int index, float value)
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

        public float GetComponent(int index)
        {
            switch (index)
            {
                case 0: return this.X;
                case 1: return this.Y;
                case 2: return this.Z;
                default: throw new IndexOutOfRangeException(String.Format("Index {0} is out of rangess", index));
            }
        }

        public Vector3 Clone()
        {
            return new Vector3(this.X, this.Y,this.Z);
        }

        public Vector3 Copy(Vector3 v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;

            return this;
        }
        public Vector3 Add(Vector3 v)
        {
            this.X += v.X;
            this.Y += v.Y;
            this.Z += v.Z;

            return this;
        }
        public Vector3 AddScalar(float s)
        {
            this.X += s;
            this.Y += s;
            this.Z += s;

            return this;
        }

        public Vector3 AddVectors(Vector3 a, Vector3 b)
        {
            this.X = a.X + b.X;
            this.Y = a.Y + b.Y;
            this.Z = a.Z + b.Z;

            return this;
        }

        public Vector3 AddScaledVector(Vector3 v, float s)
        {
            this.X += v.X * s;
            this.Y += v.Y * s;
            this.Z += v.Z * s;

            return this;
        }

        public static Vector3 operator +(Vector3 v, Vector3 w)
        {
            Vector3 r = new Vector3();
            r.X = v.X + w.X;
            r.Y = v.Y + w.Y;
            r.Z = v.Z + w.Z;

            return r;
        }

        public static Vector3 operator +(Vector3 v, float s)
        {
            Vector3 r = new Vector3();

            r.X = v.X + s;
            r.Y = v.Y + s;
            r.Z = v.Z + s;

            return r;
        }

        public Vector3 Sub(Vector3 v)
        {
            this.X -= v.X;
            this.Y -= v.Y;
            this.Z -= v.Z;

            return this;
        }

        public Vector3 SubScalar(float s)
        {
            this.X -= s;
            this.Y -= s;
            this.Z -= s;
            return this;
        }

        public Vector3 SubVectors(Vector3 a, Vector3 b)
        {
            this.X = a.X - b.X;
            this.Y = a.Y - b.Y;
            this.Z = a.Z - b.Z;

            return this;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            Vector3 r = new Vector3();

            r.X = a.X - b.X;
            r.Y = a.Y - b.Y;
            r.Z = a.Z - b.Z;
            return r;
        }

        public static Vector3 operator -(Vector3 a, float s)
        {
            Vector3 r = new Vector3(); ;
            r.X = a.X - s;
            r.Y = a.Y - s;
            r.Z = a.Z - s;

            return r;
        }
        public Vector3 Multiply(Vector3 v)
        {
            this.X *= v.X;
            this.Y *= v.Y;
            this.Z *= v.Z;

            return this;
        }

        public Vector3 MultiplyScalar(float s)
        {
            this.X *= s;
            this.Y *= s;
            this.Z *= s;

            return this;
        }

        public Vector3 MultiplyVectors(Vector3 a,Vector3 b)
        {
            this.X = a.X * b.X;
            this.Y = a.Y * b.Y;
            this.Z = a.Z * b.Z;

            return this;
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            Vector3 r = new Vector3();
            r.X = a.X * b.X;
            r.Y = a.Y * b.Y;
            r.Z = a.Z * b.Z;
            return r;
        }

        public static Vector3 operator *(Vector3 a, float s)
        {
            Vector3 r = new Vector3();
            r.X = a.X * s;
            r.Y = a.Y * s;
            r.Z = a.Z * s;
            return r;
        }
        public Vector3 ApplyEuler(Euler euler)
        {
            Quaternion quaternion = new Quaternion();
            return this.ApplyQuaternion(quaternion.SetFromEuler(euler));
        }

        public Vector3 ApplyAxisAngle(Vector3 axis,float angle)
        {
            Quaternion quaternion = new Quaternion();
            return this.ApplyQuaternion(quaternion.SetFromAxisAngle(axis, angle));
        }

        public Vector3 ApplyMatrix3(Matrix3 m)
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

        public static Vector3 operator *(Matrix3 m, Vector3 a)
        {
            var x = a.X;
            var y = a.Y;
            var z = a.Z;
            var e = m.Elements;

            Vector3 r = new Vector3();

            r.X = e[0] * x + e[3] * y + e[6] * z;
            r.Y = e[1] * x + e[4] * y + e[7] * z;
            r.Z = e[2] * x + e[5] * y + e[8] * z;

            return r;
        }

        public Vector3 ApplyNormalMatrix(Matrix3 m)
        {
            return this.ApplyMatrix3(m).Normalize();
        }

        public Vector3 ApplyMatrix4(Matrix4 m)
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

        public static Vector3 operator *(Matrix4 m, Vector3 v)
        {
            var x = v.X;
            var y = v.Y;
            var z = v.Z;
            var e = m.Elements;

            var w = 1 / (e[3] * x + e[7] * y + e[11] * z + e[15]);

            Vector3 r = new Vector3();

            r.X = (e[0] * x + e[4] * y + e[8] * z + e[12]) * w;
            r.Y = (e[1] * x + e[5] * y + e[9] * z + e[13]) * w;
            r.Z = (e[2] * x + e[6] * y + e[10] * z + e[14]) * w;

            return r;
        }

        public Vector3 ApplyQuaternion(Quaternion q)
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

        public Vector3 Project(Camera camera)
        {
            return this.ApplyMatrix4(camera.MatrixWorldInverse).ApplyMatrix4(camera.ProjectionMatrix);
        }

        public Vector3 UnProject(Camera camera)
        {
            return this.ApplyMatrix4(camera.ProjectionMatrixInverse).ApplyMatrix4(camera.MatrixWorld);
        }

        public Vector3 TransformDirection(Matrix4 m)
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

        public Vector3 Divide(Vector3 v)
        {
            this.X /= v.X;
            this.Y /= v.Y;
            this.Z /= v.Z;

            return this;
        }

        
        public Vector3 DivideScalar(float s)
        {
            return this.MultiplyScalar(1 / s);
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            Vector3 r = new Vector3();
            r.X = a.X / b.X;
            r.Y = a.Y / b.Y;
            r.Z = a.Z / b.Z;
            return r;
        }

        public static Vector3 operator /(Vector3 a, float s)
        {
            Vector3 r = new Vector3();
            r = a * (1 / s);

            return r;
        }



        public Vector3 Min(Vector3 v)
        {
            this.X = System.Math.Min(this.X, v.X);
            this.Y = System.Math.Min(this.Y, v.Y);
            this.Z = System.Math.Min(this.Z, v.Z);

            return this;
        }

        public Vector3 Max(Vector3 v)
        {
            this.X = System.Math.Max(this.X, v.X);
            this.Y = System.Math.Max(this.Y, v.Y);
            this.Z = System.Math.Max(this.Z, v.Z);

            return this;
        }

        public Vector3 Clamp(Vector3 min, Vector3 max)
        {
            this.X = System.Math.Max(min.X, System.Math.Min(max.X, this.X));
            this.Y = System.Math.Max(min.Y, System.Math.Min(max.Y, this.Y));
            this.Z = System.Math.Max(min.Z, System.Math.Min(max.Z, this.Z));

            return this;
        }

        public Vector3 ClampScalar(float minVal, float maxVal)
        {
            this.X = System.Math.Max(minVal, System.Math.Min(maxVal, this.X));
            this.Y = System.Math.Max(minVal, System.Math.Min(maxVal, this.Y));
            this.Z = System.Math.Max(minVal, System.Math.Min(maxVal, this.Z));

            return this;
        }

        public bool Equals(Vector3 v)
        {
            return this.X == v.X && this.Y == v.Y && this.Z == v.Z;
        }

        public Vector3 ClampLength(float min, float max)
        {
            var length = this.Length();

            return this.DivideScalar(length != 0 ? length : 1).MultiplyScalar(System.Math.Max(min, System.Math.Min(max, length)));
        }

        public Vector3 Floor()
        {
            this.X = (float)System.Math.Floor(this.X);
            this.Y = (float)System.Math.Floor(this.Y);
            this.Z = (float)System.Math.Floor(this.Z);

            return this;
        }

        public Vector3 Ceil()
        {
            this.X = (float)System.Math.Ceiling(this.X);
            this.Y = (float)System.Math.Ceiling(this.Y);
            this.Z = (float)System.Math.Ceiling(this.Z);

            return this;
        }

        public Vector3 Round()
        {
            this.X = (float)System.Math.Round(this.X);
            this.Y = (float)System.Math.Round(this.Y);
            this.Z = (float)System.Math.Round(this.Z);

            return this;
        }

        public Vector3 RoundToZero()
        {
            this.X = (this.X < 0) ? (float)System.Math.Ceiling(this.X) : (float)System.Math.Floor(this.X);
            this.Y = (this.Y < 0) ? (float)System.Math.Ceiling(this.Y) : (float)System.Math.Floor(this.Y);
            this.Z = (this.Z < 0) ? (float)System.Math.Ceiling(this.Z) : (float)System.Math.Floor(this.Z);

            return this;
        }

        public Vector3 Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;

            return this;
        }

        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return v1.Dot(v2);
        }
        public float Dot(Vector3 v)
        {
            return this.X * v.X + this.Y * v.Y + this.Z*v.Z;
        }
        
        public float LengthSq()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        public float Length()
        {
            return (float)System.Math.Sqrt(this.LengthSq());
        }

        public float ManhattanLength()
        {
            return (float)(System.Math.Abs(this.X) + System.Math.Abs(this.Y)+System.Math.Abs(this.Z));
        }

        public Vector3 Normalize()
        {
            return this.DivideScalar(this.Length() != 0 ? this.Length() : 1);
        }
        
        public Vector3 SetLength(float length)
        {
            return this.Normalize().MultiplyScalar(length);
        }

        public Vector3 Lerp(Vector3 v, float alpha)
        {
            this.X += (v.X - this.X) * alpha;
            this.Y += (v.Y - this.Y) * alpha;
            this.Z += (v.Z - this.Z) * alpha;

            return this;
        }

        public Vector3 LerpVectors(Vector3 v1, Vector3 v2, float alpha)
        {
            return this.SubVectors(v2, v1).MultiplyScalar(alpha).Add(v1);
        }

        public Vector3 Cross(Vector3 v)
        {
            return this.CrossVectors(this, v);
        }

        public Vector3 CrossVectors(Vector3 a, Vector3 b)
        {
            float ax = a.X, ay = a.Y, az = a.Z;
            float bx = b.X, by = b.Y, bz = b.Z;

            this.X = ay * bz - az * by;
            this.Y = az * bx - ax * bz;
            this.Z = ax * by - ay * bx;

            return this;
        }

        public Vector3 ProjectOnVector(Vector3 v)
        {
            var scalar = v.Dot(this) / v.LengthSq();

            return this.Copy(v).MultiplyScalar(scalar);
            // return v*scalar;
        }

        public Vector3 ProjectOnPlane(Vector3 planeNormal)
        {
            Vector3 _vector = Vector3.Zero();
            _vector.Copy(this).ProjectOnVector(planeNormal);

            return this.Sub(_vector);
        }

        public Vector3 Reflect(Vector3 normal)
        {
            // reflect incident vector off plane orthogonal to normal
            // normal is assumed to have unit length
            Vector3 _vector = Vector3.Zero();
            return this.Sub(_vector.Copy(normal).MultiplyScalar(2 * this.Dot(normal)));
        }

        public float AngleTo(Vector3 v)
        {
           var denominator = System.Math.Sqrt( this.LengthSq() * v.LengthSq() );

		    if ( denominator == 0 )
            {
                throw new Exception("THREE.Math.Vector3: AngleTo() can\'t handle zero length vectors.");
            }

		    float theta = (float)(this.Dot( v ) / denominator);

		    // clamp, to handle numerical problems

		    return (float)System.Math.Acos(Clamp(theta,-1,1));
        }

        public float Clamp(float value, float min, float max)
        {
            return (float)System.Math.Max(min, System.Math.Min(max, value));
        }

        public float DistanceTo(Vector3 v)
        {
            return (float)System.Math.Sqrt(this.DistanceToSquared(v));
        }

        public float DistanceToSquared(Vector3 v)
        {
            var dx = this.X - v.X;
            var dy = this.Y - v.Y;
            var dz = this.Z - v.Z;

            return dx * dx + dy * dy + dz * dz;
        }

        public float ManhattanDistanceTo(Vector3 v)
        {
            return (float)(System.Math.Abs(this.X - v.X) + System.Math.Abs(this.Y - v.Y) + System.Math.Abs(this.Z - v.Z));
        }

        public Vector3 SetFromSpherical(Spherical s)
        {
            return this.SetFromSphericalCoords(s.Radius, s.Phi, s.Theta);
        }

        public Vector3 SetFromSphericalCoords(float radius, float phi, float theta)
        {

            var sinPhiRadius = System.Math.Sin(phi) * radius;

            this.X = (float)(sinPhiRadius * System.Math.Sin(theta));
            this.Y = (float)(System.Math.Cos(phi) * radius);
            this.Z = (float)(sinPhiRadius * System.Math.Cos(theta));

            return this;
        }

        public Vector3 SetFromCylindrical(Cylindrical c)
        {
            return this.SetFromSphericalCoords(c.Radius, c.Theta, c.Y);
        }

        public Vector3 SetFromCylindricalCoords(float radius, float theta, float y)
        {
            this.X = (float)(radius * System.Math.Sin(theta));
            this.Y = y;
            this.Z = (float)(radius * System.Math.Cos(theta));

            return this;
        }

        public Vector3 SetFromMatrixPosition(Matrix4 m)
        {
            var e = m.Elements;

            this.X = e[12];
            this.Y = e[13];
            this.Z = e[14];

            return this;
        }

        public Vector3 SetFromMatrixScale(Matrix4 m)
        {
            var sx = this.SetFromMatrixColumn(m, 0).Length();
            var sy = this.SetFromMatrixColumn(m, 1).Length();
            var sz = this.SetFromMatrixColumn(m, 2).Length();

            this.X = sx;
            this.Y = sy;
            this.Z = sz;

            return this;
        }

        public Vector3 SetFromMatrixColumn(Matrix4 m, int index)
        {
            return this.FromArray(m.Elements, index * 4);
        }

        public Vector3 FromArray(float[] array, int? offset = null)
        {
            int index = 0;
            if (offset != null) index = offset.Value;
            int aLen = array.Length - 1;
            this.X = index<=aLen ? array[index] : float.NaN;
            this.Y = index<=aLen? array[index + 1] : float.NaN;
            this.Z = index<=aLen? array[index + 2] : float.NaN;

            return this;
        }

        public float[] ToArray(float[] array=null, int? offset = null)
        {
            int index = 0;
            if (array == null) array = new float[3];
            if (offset != null) index = offset.Value;

            array[index] = this.X;
            array[index + 1] = this.Y;
            array[index + 2] = this.Z;

            return array;
        }

        public Vector3 FromBufferAttribute(BufferAttribute<float> attribute, int index)
        {
            this.X = attribute.getX(index);
            this.Y = attribute.getY(index);
            this.Z = attribute.getZ(index);

            return this;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public Vector3d ToVector3d()
        {
            return new Vector3d((double)X, (double)Y, (double)Z);
        }

        public Vector4 ToVector4()
        {
            return new Vector4(X, Y, Z, 1);
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
