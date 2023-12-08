using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace THREE
{
    public class Vector4d : IEquatable<Vector4d>, ICloneable, INotifyPropertyChanged
    {
        public double X;

        public double Y;

        public double Z;

        public double W;

        public Vector4d()
        {
            this.X = this.Y = this.Z = 0;
            this.W = 1;
        }

        public Vector4d(double x, double y, double z,double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public static Vector4d Zero()
        {
            return new Vector4d(0, 0, 0, 0);
        }

        public static Vector4d One()
        {
            return new Vector4d(1, 1, 1, 1);
        }

        public Vector4d Set(double x, double y, double z,double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;

            return this;
        }

        public Vector4d SetScalar(double scalar)
        {
            this.X = scalar;
            this.Y = scalar;
            this.Z = scalar;
            this.W = scalar;

            return this;
        }

        public Vector4d SetX(double x)
        {
            this.X = x;
            return this;
        }

        public Vector4d SetY(double y)
        {
            this.Y = y;

            return this;
        }

        public Vector4d SetZ(double z)
        {
            this.Z = z;

            return this;
        }

        public Vector4d SetW(double w)
        {
            this.W = w;

            return this;
        }

        public Vector4d SetComponent(int index, double value)
        {
            switch (index)
            {
                case 0: this.X = value; break;
                case 1: this.Y = value; break;
                case 2: this.Z = value; break;
                case 3: this.W = value; break;
                default: throw new IndexOutOfRangeException(String.Format("Index {0} is out of rangess", index));
            }
            return this;
        }

        public double GetComponent(int index, double value)
        {
            switch (index)
            {
                case 0: return this.X;
                case 1: return this.Y;
                case 2: return this.Z;
                case 3: return this.W;
                default: throw new IndexOutOfRangeException(String.Format("Index {0} is out of rangess", index));
            }
        }

        public object Clone()
        {
            return new Vector4d(this.X, this.Y, this.Z,this.W);
        }

        public Vector4d Copy(Vector4d v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            this.W = v.W;

            return this;
        }
        public Vector4d Add(Vector4d v)
        {
            this.X += v.X;
            this.Y += v.Y;
            this.Z += v.Z;
            this.W += v.W;
            return this;
        }
        public Vector4d AddScalar(double s)
        {
            this.X += s;
            this.Y += s;
            this.Z += s;
            this.W += s;
            return this;
        }

        public Vector4d AddVectors(Vector4d a, Vector4d b)
        {
            this.X = a.X + b.X;
            this.Y = a.Y + b.Y;
            this.Z = a.Z + b.Z;
            this.W = a.W + b.W;

            return this;
        }

        public Vector4d AddScaledVector(Vector4d v, double s)
        {
            this.X += v.X * s;
            this.Y += v.Y * s;
            this.Z += v.Z * s;
            this.W += v.W * s;

            return this;
        }

        public static Vector4d operator +(Vector4d v, Vector4d w)
        {
            Vector4d r = new Vector4d();
            r.X = v.X + w.X;
            r.Y = v.Y + w.Y;
            r.Z = v.Z + w.Z;
            r.W = v.W + w.W;

            return r;
        }

        public static Vector4d operator +(Vector4d v, double s)
        {
            Vector4d r = new Vector4d();

            r.X = v.X + s;
            r.Y = v.Y + s;
            r.Z = v.Z + s;
            r.W = v.W + s;

            return r;
        }

        public Vector4d Sub(Vector4d v)
        {
            this.X -= v.X;
            this.Y -= v.Y;
            this.Z -= v.Z;
            this.W -= v.W;

            return this;
        }

        public Vector4d SubScalar(double s)
        {
            this.X -= s;
            this.Y -= s;
            this.Z -= s;
            this.W -= s;

            return this;
        }

        public Vector4d SubVectors(Vector4d a, Vector4d b)
        {
            this.X = a.X - b.X;
            this.Y = a.Y - b.Y;
            this.Z = a.Z - b.Z;
            this.W = a.W - b.W;

            return this;
        }

        public static Vector4d operator -(Vector4d a, Vector4d b)
        {
            Vector4d r = new Vector4d();

            r.X = a.X - b.X;
            r.Y = a.Y - b.Y;
            r.Z = a.Z - b.Z;
            r.W = a.W - b.W;

            return r;
        }

        public static Vector4d operator -(Vector4d a, double s)
        {
            Vector4d r = new Vector4d(); ;
            r.X = a.X - s;
            r.Y = a.Y - s;
            r.Z = a.Z - s;
            r.W = a.W - s;

            return r;
        }
        public Vector4d Multiply(Vector4d v)
        {
            this.X *= v.X;
            this.Y *= v.Y;
            this.Z *= v.Z;
            this.W *= v.W;

            return this;
        }

        public Vector4d MultiplyScalar(double s)
        {
            this.X *= s;
            this.Y *= s;
            this.Z *= s;
            this.W *= s;

            return this;
        }

        public Vector4d MultiplyVectors(Vector4d a, Vector4d b)
        {
            this.X = a.X * b.X;
            this.Y = a.Y * b.Y;
            this.Z = a.Z * b.Z;
            this.W = a.W * b.W;

            return this;
        }

        public static Vector4d operator *(Vector4d a, Vector4d b)
        {
            Vector4d r = new Vector4d();
            r.X = a.X * b.X;
            r.Y = a.Y * b.Y;
            r.Z = a.Z * b.Z;
            r.W = a.W * b.W;

            return r;
        }

        public static Vector4d operator *(Vector4d a, double s)
        {
            Vector4d r = new Vector4d();
            r.X = a.X * s;
            r.Y = a.Y * s;
            r.Z = a.Z * s;
            r.W = a.W * s;

            return r;
        }

        public Vector4d ApplyMatrix4(Matrix4d m)
        {
            double x = this.X, y = this.Y, z = this.Z, w = this.W;
            var e = m.Elements;

            this.X = e[0] * x + e[4] * y + e[8] * z + e[12] * w;
            this.Y = e[1] * x + e[5] * y + e[9] * z + e[13] * w;
            this.Z = e[2] * x + e[6] * y + e[10] * z + e[14] * w;
            this.W = e[3] * x + e[7] * y + e[11] * z + e[15] * w;

            return this;
        }

        public static Vector4d operator *(Matrix4d m, Vector4d a)
        {
            var x = a.X;
            var y = a.Y;
            var z = a.Z;
            var w = a.W;
            var e = m.Elements;

            Vector4d r = new Vector4d();

            r.X = e[0] * x + e[4] * y + e[8] * z + e[12] * w;
            r.Y = e[1] * x + e[5] * y + e[9] * z + e[13] * w;
            r.Z = e[2] * x + e[6] * y + e[10] * z + e[14] * w;
            r.W = e[3] * x + e[7] * y + e[11] * z + e[15] * w;

            return r;
        }

        public Vector4d Divide(Vector4d v)
        {
            this.X /= v.X;
            this.Y /= v.Y;
            this.Z /= v.Z;
            this.W /= v.W;

            return this;
        }

        public Vector4d DivideScalar(double s)
        {
            return this.MultiplyScalar(1 / s);
        }

        public static Vector4d operator /(Vector4d a, Vector4d b)
        {
            Vector4d r = new Vector4d();
            r.X = a.X / b.X;
            r.Y = a.Y / b.Y;
            r.Z = a.Z / b.Z;
            r.W = a.W / b.W;

            return r;
        }

        public static Vector4d operator /(Vector4d a, double s)
        {
            Vector4d r = new Vector4d();
            r = a * (1 / s);

            return r;
        }

        public Vector4d SetAxisAngleFromQuaternion(Quaterniond q ) {

		    // http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToAngle/index.htm

		    // q is assumed to be normalized

		    this.W = 2 * System.Math.Acos( q.W );

		    var s = System.Math.Sqrt( 1 - q.W * q.W );

		    if ( s < 0.0001 ) {

			    this.X = 1;
			    this.Y = 0;
			    this.Z = 0;

		    } else {

			    this.X = q.X / s;
			    this.Y = q.Y / s;
			    this.Z = q.Z / s;

		    }

		    return this;

	    }

	    public Vector4d SetAxisAngleFromRotationMatrix(Matrix4d m ) {

		    // http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToAngle/index.htm

		    // assumes the upper 3x3 of m is a pure rotation matrix (i.e, unscaled)

		    double angle, x, y, z,		// variables for result
			    epsilon = 0.01f,		// margin to allow for rounding errors
			    epsilon2 = 0.1f;		// margin to distinguish between 0 and 180 degrees

			double[] te = m.Elements;

			double m11 = te[ 0 ], m12 = te[ 4 ], m13 = te[ 8 ],
			      m21 = te[ 1 ], m22 = te[ 5 ], m23 = te[ 9 ],
			      m31 = te[ 2 ], m32 = te[ 6 ], m33 = te[ 10 ];

		    if ( ( System.Math.Abs( m12 - m21 ) < epsilon ) &&
		         ( System.Math.Abs( m13 - m31 ) < epsilon ) &&
		         ( System.Math.Abs( m23 - m32 ) < epsilon ) ) {

			    // singularity found
			    // first check for identity matrix which must have +1 for all terms
			    // in leading diagonal and zero in other terms

			    if ( ( System.Math.Abs( m12 + m21 ) < epsilon2 ) &&
			         ( System.Math.Abs( m13 + m31 ) < epsilon2 ) &&
			         ( System.Math.Abs( m23 + m32 ) < epsilon2 ) &&
			         ( System.Math.Abs( m11 + m22 + m33 - 3 ) < epsilon2 ) ) {

				    // this singularity is identity matrix so angle = 0

				    this.Set( 1, 0, 0, 0 );

				    return this; // zero angle, arbitrary axis

			    }

			    // otherwise this singularity is angle = 180

			    angle = System.Math.PI;

			    var xx = ( m11 + 1 ) / 2;
			    var yy = ( m22 + 1 ) / 2;
			    var zz = ( m33 + 1 ) / 2;
			    var xy = ( m12 + m21 ) / 4;
			    var xz = ( m13 + m31 ) / 4;
			    var yz = ( m23 + m32 ) / 4;

			    if ( ( xx > yy ) && ( xx > zz ) ) {

				    // m11 is the largest diagonal term

				    if ( xx < epsilon ) {

					    x = 0;
					    y = 0.707106781f;
					    z = 0.707106781f;

				    } else {

					    x = System.Math.Sqrt( xx );
					    y = xy / x;
					    z = xz / x;

				    }

			    } else if ( yy > zz ) {

				    // m22 is the largest diagonal term

				    if ( yy < epsilon ) {

					    x = 0.707106781f;
					    y = 0;
					    z = 0.707106781f;

				    } else {

					    y = System.Math.Sqrt( yy );
					    x = xy / y;
					    z = yz / y;

				    }

			    } else {

				    // m33 is the largest diagonal term so base result on this

				    if ( zz < epsilon ) {

					    x = 0.707106781f;
					    y = 0.707106781f;
					    z = 0;

				    } else {

					    z = System.Math.Sqrt( zz );
					    x = xz / z;
					    y = yz / z;

				    }

			    }

			    this.Set( x, y, z, angle );

			    return this; // return 180 deg rotation

		    }

		    // as we have reached here there are no singularities so we can handle normally

		    var s = System.Math.Sqrt( ( m32 - m23 ) * ( m32 - m23 ) +
		                       ( m13 - m31 ) * ( m13 - m31 ) +
		                       ( m21 - m12 ) * ( m21 - m12 ) ); // used to normalize

		    if ( System.Math.Abs( s ) < 0.001 ) s = 1;

		    // prevent divide by zero, should not happen if matrix is orthogonal and should be
		    // caught by singularity test above, but I've left it in just in case

		    this.X = ( m32 - m23 ) / s;
		    this.Y = ( m13 - m31 ) / s;
		    this.Z = ( m21 - m12 ) / s;
		    this.W = System.Math.Acos( ( m11 + m22 + m33 - 1 ) / 2 );

		    return this;

	    }
       
        public Vector4d Min(Vector4d v)
        {
            this.X = System.Math.Min(this.X, v.X);
            this.Y = System.Math.Min(this.Y, v.Y);
            this.Z = System.Math.Min(this.Z, v.Z);
            this.W = System.Math.Min(this.W, v.W);
            return this;
        }

        public Vector4d Max(Vector4d v)
        {
            this.X = System.Math.Max(this.X, v.X);
            this.Y = System.Math.Max(this.Y, v.Y);
            this.Z = System.Math.Max(this.Z, v.Z);
            this.W = System.Math.Max(this.W, v.W);

            return this;
        }

        public Vector4d Clamp(Vector4d min, Vector4d max)
        {
            this.X = System.Math.Max(min.X, System.Math.Min(max.X, this.X));
            this.Y = System.Math.Max(min.Y, System.Math.Min(max.Y, this.Y));
            this.Z = System.Math.Max(min.Z, System.Math.Min(max.Z, this.Z));
            this.W = System.Math.Max(min.W, System.Math.Min(max.W, this.W));

            return this;
        }

        public Vector4d ClampScalar(double minVal, double maxVal)
        {
            this.X = System.Math.Max(minVal, System.Math.Min(maxVal, this.X));
            this.Y = System.Math.Max(minVal, System.Math.Min(maxVal, this.Y));
            this.Z = System.Math.Max(minVal, System.Math.Min(maxVal, this.Z));
            this.W = System.Math.Max(minVal, System.Math.Min(maxVal, this.W));

            return this;
        }

        public Vector4d ClampLength(double min, double max)
        {
            var length = this.Length();

            return this.DivideScalar(length != 0 ? length : 1).MultiplyScalar(System.Math.Max(min, System.Math.Min(max, length)));
        }

        public bool Equals(Vector4d v)
        {
            return this.X == v.X && this.Y == v.Y && this.Z == v.Z && this.W == v.W;
        }

        public Vector4d Floor()
        {
            this.X = System.Math.Floor(this.X);
            this.Y = System.Math.Floor(this.Y);
            this.Z = System.Math.Floor(this.Z);
            this.W = System.Math.Floor(this.W);

            return this;
        }

        public Vector4d Ceil()
        {
            this.X = System.Math.Ceiling(this.X);
            this.Y = System.Math.Ceiling(this.Y);
            this.Z = System.Math.Ceiling(this.Z);
            this.W = System.Math.Ceiling(this.W);

            return this;
        }

        public Vector4d Round()
        {
            this.X = System.Math.Round(this.X);
            this.Y = System.Math.Round(this.Y);
            this.Z = System.Math.Round(this.Z);
            this.W = System.Math.Round(this.W);

            return this;
        }

        public Vector4d RoundToZero()
        {
            this.X = (this.X < 0) ? System.Math.Ceiling(this.X) : System.Math.Floor(this.X);
            this.Y = (this.Y < 0) ? System.Math.Ceiling(this.Y) : System.Math.Floor(this.Y);
            this.Z = (this.Z < 0) ? System.Math.Ceiling(this.Z) : System.Math.Floor(this.Z);
            this.W = (this.W < 0) ? System.Math.Ceiling(this.W) : System.Math.Floor(this.W);

            return this;
        }

        public Vector4d Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
            this.W = -this.W;

            return this;
        }

        public double Dot(Vector4d v)
        {
            return this.X * v.X + this.Y * v.Y + this.Z * v.Z + this.W * v.W;
        }

        public double LengthSq()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
        }

        public double Length()
        {
            return System.Math.Sqrt(this.LengthSq());
        }

        public double ManhattanLength()
        {
            return (System.Math.Abs(this.X) + System.Math.Abs(this.Y) + System.Math.Abs(this.Z) + System.Math.Abs(this.W));
        }

        public Vector4d Normalize()
        {
            return this.DivideScalar(this.Length() != 0 ? this.Length() : 1);
        }

        public Vector4d SetLength(double length)
        {
            return this.Normalize().MultiplyScalar(length);
        }

        public Vector4d Lerp(Vector4d v, double alpha)
        {
            this.X += (v.X - this.X) * alpha;
            this.Y += (v.Y - this.Y) * alpha;
            this.Z += (v.Z - this.Z) * alpha;
            this.W += (v.W - this.W) * alpha;

            return this;
        }

        public Vector4d LerpVectors(Vector4d v1, Vector4d v2, double alpha)
        {
            return this.SubVectors(v2, v1).MultiplyScalar(alpha).Add(v1);
        }

        public Vector4d FromArray(double[] array, int? offset = null)
        {
            int index = 0;
            if (offset != null) index = offset.Value;

            this.X = array[index];
            this.Y = array[index + 1];
            this.Z = array[index + 2];
            this.W = array[index + 3];

            return this;
        }

        public double[] ToArray(double[] array=null, int? offset = null)
        {
            int index = 0;
            if (array == null) array = new double[4];
            if (offset != null) index = offset.Value;

            array[index] = this.X;
            array[index + 1] = this.Y;
            array[index + 2] = this.Z;
            array[index + 3] = this.W;

            return array;
        }

        public Vector4d FromBufferAttribute(BufferAttribute<double> attribute, int index)
        {
            this.X = attribute.getX(index);
            this.Y = attribute.getY(index);
            this.Z = attribute.getZ(index);
            this.W = attribute.getW(index);

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
