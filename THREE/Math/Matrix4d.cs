using System.Diagnostics;


namespace THREE
{
    public class Matrix4d
    {
        public double[] Elements = new double[16] { 1, 0, 0, 0,
                                                  0, 1, 0, 0,
                                                  0, 0, 1, 0,
                                                  0, 0, 0, 1
                                                 };
        public Matrix4d()
        {
        }

        public Matrix4d( double n11, double n12, double n13,double n14,
                        double n21, double n22, double n23,double n24,
                        double n31, double n32, double n33,double n34,
                        double n41, double n42, double n43,double n44)
        {
            var te = this.Elements;

            te[0] = n11; te[4] = n12; te[8] = n13; te[12] = n14;
            te[1] = n21; te[5] = n22; te[9] = n23; te[13] = n24;
            te[2] = n31; te[6] = n32; te[10] = n33; te[14] = n34;
            te[3] = n41; te[7] = n42; te[11] = n43; te[15] = n44;
        }

        public Matrix4d Set(double n11, double n12, double n13, double n14,
                        double n21, double n22, double n23, double n24,
                        double n31, double n32, double n33, double n34,
                        double n41, double n42, double n43, double n44)
        {
            var te = this.Elements;

            te[0] = n11; te[4] = n12; te[8] = n13; te[12] = n14;
            te[1] = n21; te[5] = n22; te[9] = n23; te[13] = n24;
            te[2] = n31; te[6] = n32; te[10] = n33; te[14] = n34;
            te[3] = n41; te[7] = n42; te[11] = n43; te[15] = n44;

            return this;
        }
        public static Matrix4d Identity()
        {
            return new Matrix4d();
        }
        public Matrix4d Copy(Matrix4d m)
        {
            var te = this.Elements;
            var me = m.Elements;

            te[0] = me[0]; te[1] = me[1]; te[2] = me[2]; te[3] = me[3];
            te[4] = me[4]; te[5] = me[5]; te[6] = me[6]; te[7] = me[7];
            te[8] = me[8]; te[9] = me[9]; te[10] = me[10]; te[11] = me[11];
            te[12] = me[12]; te[13] = me[13]; te[14] = me[14]; te[15] = me[15];

            return this;
        }
        public Matrix4 ToMatrix4f()
        {
            var te = Elements.ToFloatArray();

            var m = new Matrix4().FromArray(te); ;

            return m;
        }
        public object Clone()
        {
            return new Matrix4d().FromArray(this.Elements);
        }

        public Matrix4d CopyPosition(Matrix4d m)
        {
            var te = this.Elements;
            var me = m.Elements;

            te[12] = me[12];
            te[13] = me[13];
            te[14] = me[14];

            return this;
        }

        public Matrix4d ExtractBasis(Vector3d xAxis, Vector3d yAxis, Vector3d zAxis)
        {
            xAxis.SetFromMatrixColumn(this, 0);
            yAxis.SetFromMatrixColumn(this, 1);
            zAxis.SetFromMatrixColumn(this, 2);

            return this;
        }

        public Matrix4d MakeBasis(Vector3d xAxis, Vector3d yAxis, Vector3d zAxis)
        {
            this.Set(
                xAxis.X, yAxis.X, zAxis.X, 0,
                xAxis.Y, yAxis.Y, zAxis.Y, 0,
                xAxis.Z, yAxis.Z, zAxis.Z, 0,
                0, 0, 0, 1);

            return this;
        }

        public Matrix4d ExtractRotation(Matrix4d m)
        {
            // this method does not support reflection matrices

            var te = this.Elements;
            var me = m.Elements;
            Vector3d _v1 = new Vector3d();

            var scaleX = 1 / _v1.SetFromMatrixColumn(m, 0).Length();
            var scaleY = 1 / _v1.SetFromMatrixColumn(m, 1).Length();
            var scaleZ = 1 / _v1.SetFromMatrixColumn(m, 2).Length();

            te[0] = me[0] * scaleX;
            te[1] = me[1] * scaleX;
            te[2] = me[2] * scaleX;
            te[3] = 0;

            te[4] = me[4] * scaleY;
            te[5] = me[5] * scaleY;
            te[6] = me[6] * scaleY;
            te[7] = 0;

            te[8] = me[8] * scaleZ;
            te[9] = me[9] * scaleZ;
            te[10] = me[10] * scaleZ;
            te[11] = 0;

            te[12] = 0;
            te[13] = 0;
            te[14] = 0;
            te[15] = 1;

            return this;
        }

        public Matrix4d MakeRotationFromEuler(Euler euler)
        {
            var te = this.Elements;

		    double x = euler.X, y = euler.Y, z = euler.Z;
		    double a = System.Math.Cos( x ), b =  System.Math.Sin( x );
		    double c = System.Math.Cos( y ), d =  System.Math.Sin( y );
		    double e =  System.Math.Cos( z ), f =  System.Math.Sin( z );

		    if ( euler.Order == RotationOrder.XYZ ) 
            {

			    double ae = a * e, af = a * f, be = b * e, bf = b * f;

			    te[ 0 ] = c * e;
			    te[ 4 ] = - c * f;
			    te[ 8 ] = d;

			    te[ 1 ] = af + be * d;
			    te[ 5 ] = ae - bf * d;
			    te[ 9 ] = - b * c;

			    te[ 2 ] = bf - ae * d;
			    te[ 6 ] = be + af * d;
			    te[ 10 ] = a * c;

		    } 
            else if ( euler.Order == RotationOrder.YXZ ) 
            {

			    double ce = c * e, cf = c * f, de = d * e, df = d * f;

			    te[ 0 ] = ce + df * b;
			    te[ 4 ] = de * b - cf;
			    te[ 8 ] = a * d;

			    te[ 1 ] = a * f;
			    te[ 5 ] = a * e;
			    te[ 9 ] = - b;

			    te[ 2 ] = cf * b - de;
			    te[ 6 ] = df + ce * b;
			    te[ 10 ] = a * c;

		    } 
            else if ( euler.Order == RotationOrder.ZXY) 
            {

			    double ce = c * e, cf = c * f, de = d * e, df = d * f;

			    te[ 0 ] = ce - df * b;
			    te[ 4 ] = - a * f;
			    te[ 8 ] = de + cf * b;

			    te[ 1 ] = cf + de * b;
			    te[ 5 ] = a * e;
			    te[ 9 ] = df - ce * b;

			    te[ 2 ] = - a * d;
			    te[ 6 ] = b;
			    te[ 10 ] = a * c;

		    } 
            else if ( euler.Order == RotationOrder.ZYX) {

			    double ae = a * e, af = a * f, be = b * e, bf = b * f;

			    te[ 0 ] = c * e;
			    te[ 4 ] = be * d - af;
			    te[ 8 ] = ae * d + bf;

			    te[ 1 ] = c * f;
			    te[ 5 ] = bf * d + ae;
			    te[ 9 ] = af * d - be;

			    te[ 2 ] = - d;
			    te[ 6 ] = b * c;
			    te[ 10 ] = a * c;

		    } 
            else if ( euler.Order == RotationOrder.YZX) 
            {

			    double ac = a * c, ad = a * d, bc = b * c, bd = b * d;

			    te[ 0 ] = c * e;
			    te[ 4 ] = bd - ac * f;
			    te[ 8 ] = bc * f + ad;

			    te[ 1 ] = f;
			    te[ 5 ] = a * e;
			    te[ 9 ] = - b * e;

			    te[ 2 ] = - d * e;
			    te[ 6 ] = ad * f + bc;
			    te[ 10 ] = ac - bd * f;

		    } 
            else if ( euler.Order == RotationOrder.XZY ) {

			    double ac = a * c, ad = a * d, bc = b * c, bd = b * d;

			    te[ 0 ] = c * e;
			    te[ 4 ] = - f;
			    te[ 8 ] = d * e;

			    te[ 1 ] = ac * f + bd;
			    te[ 5 ] = a * e;
			    te[ 9 ] = ad * f - bc;

			    te[ 2 ] = bc * f - ad;
			    te[ 6 ] = b * e;
			    te[ 10 ] = bd * f + ac;

		    }

		    // bottom row
		    te[ 3 ] = 0;
		    te[ 7 ] = 0;
		    te[ 11 ] = 0;

		    // last column
		    te[ 12 ] = 0;
		    te[ 13 ] = 0;
		    te[ 14 ] = 0;
		    te[ 15 ] = 1;

		    return this;

        }

        private Vector3d _zero = Vector3d.Zero();

        private Vector3d _one = Vector3d.One();

        private Vector3d _x = new Vector3d();

        private Vector3d _y = new Vector3d();

        private Vector3d _z = new Vector3d();

        public Matrix4d MakeRotationFromQuaternion(Quaterniond q)
        {
		    return this.Compose( _zero, q, _one );
	    }
                
        public Matrix4d LookAt(Vector3d eye, Vector3d target, Vector3d up)
        {
            var te = this.Elements;

		    _z.SubVectors( eye, target );

		    if ( _z.LengthSq() == 0 ) 
            {
			    // eye and target are in the same position
			    _z.Z = 1;
		    }

		    _z.Normalize();
		    _x.CrossVectors( up, _z );

		    if ( _x.LengthSq() == 0 ) {
			    // up and z are parallel
			    if ( System.Math.Abs( up.Z ) == 1 ) {

				    _z.X += 0.0001f;

			    } else {

				    _z.Z += 0.0001f;

			    }

			    _z.Normalize();
			    _x.CrossVectors( up, _z );

		    }

		    _x.Normalize();
		    _y.CrossVectors( _z, _x );

		    te[ 0 ] = _x.X; te[ 4 ] = _y.X; te[ 8 ] = _z.X;
		    te[ 1 ] = _x.Y; te[ 5 ] = _y.Y; te[ 9 ] = _z.Y;
		    te[ 2 ] = _x.Z; te[ 6 ] = _y.Z; te[ 10 ] = _z.Z;

		    return this;
        }
        public Matrix4d Multiply(Matrix4d m)
        {
            return this.MultiplyMatrices(this, m);
        }

        public Matrix4d PreMultiply(Matrix4d m)
        {
            return this.MultiplyMatrices(m, this);
        }

        public Matrix4d MultiplyMatrices(Matrix4d a, Matrix4d b)
        {
            var ae = a.Elements;
            var be = b.Elements;
            var te = this.Elements;

            double a11 = ae[0], a12 = ae[4], a13 = ae[8], a14 = ae[12];
            double a21 = ae[1], a22 = ae[5], a23 = ae[9], a24 = ae[13];
            double a31 = ae[2], a32 = ae[6], a33 = ae[10], a34 = ae[14];
            double a41 = ae[3], a42 = ae[7], a43 = ae[11], a44 = ae[15];

            double b11 = be[0], b12 = be[4], b13 = be[8], b14 = be[12];
            double b21 = be[1], b22 = be[5], b23 = be[9], b24 = be[13];
            double b31 = be[2], b32 = be[6], b33 = be[10], b34 = be[14];
            double b41 = be[3], b42 = be[7], b43 = be[11], b44 = be[15];

            te[0] = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
            te[4] = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
            te[8] = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
            te[12] = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;

            te[1] = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
            te[5] = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
            te[9] = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
            te[13] = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;

            te[2] = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
            te[6] = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
            te[10] = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
            te[14] = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;

            te[3] = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
            te[7] = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
            te[11] = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
            te[15] = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;

            return this;
        }

        public static Matrix4d operator *(Matrix4d a, Matrix4d b)
        {
            Matrix4d r = Matrix4d.Identity();
            var ae = a.Elements;
            var be = b.Elements;
            var te = r.Elements;

            double a11 = ae[0], a12 = ae[4], a13 = ae[8], a14 = ae[12];
            double a21 = ae[1], a22 = ae[5], a23 = ae[9], a24 = ae[13];
            double a31 = ae[2], a32 = ae[6], a33 = ae[10], a34 = ae[14];
            double a41 = ae[3], a42 = ae[7], a43 = ae[11], a44 = ae[15];

            double b11 = be[0], b12 = be[4], b13 = be[8], b14 = be[12];
            double b21 = be[1], b22 = be[5], b23 = be[9], b24 = be[13];
            double b31 = be[2], b32 = be[6], b33 = be[10], b34 = be[14];
            double b41 = be[3], b42 = be[7], b43 = be[11], b44 = be[15];

            te[0] = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
            te[4] = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
            te[8] = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
            te[12] = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;

            te[1] = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
            te[5] = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
            te[9] = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
            te[13] = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;

            te[2] = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
            te[6] = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
            te[10] = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
            te[14] = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;

            te[3] = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
            te[7] = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
            te[11] = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
            te[15] = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;

            return r;
        }

        public Matrix4d MultiplyScalar(double s)
        {
            var te = this.Elements;

            te[0] *= s; te[4] *= s; te[8] *= s; te[12] *= s;
            te[1] *= s; te[5] *= s; te[9] *= s; te[13] *= s;
            te[2] *= s; te[6] *= s; te[10] *= s; te[14] *= s;
            te[3] *= s; te[7] *= s; te[11] *= s; te[15] *= s;

            return this;
        }

        public BufferAttribute<double> ApplyToBufferAttribute(BufferAttribute<double> attribute)
        {
            Vector3d _v1 = Vector3d.Zero();
            for (int i = 0, l = attribute.count; i < l; i++)
            {

                _v1.X = attribute.getX(i);
                _v1.Y = attribute.getY(i);
                _v1.Z = attribute.getZ(i);

                _v1.ApplyMatrix4(this);

                attribute.setXYZ(i, _v1.X, _v1.Y, _v1.Z);

            }

            return attribute;
        }

        public double Determinant()
        {
            var te = this.Elements;

            double n11 = te[0], n12 = te[4], n13 = te[8], n14 = te[12];
            double n21 = te[1], n22 = te[5], n23 = te[9], n24 = te[13];
            double n31 = te[2], n32 = te[6], n33 = te[10], n34 = te[14];
            double n41 = te[3], n42 = te[7], n43 = te[11], n44 = te[15];

            //TODO: make this more efficient
            //( based on http://www.euclideanspace.com/maths/algebra/matrix/functions/inverse/fourD/index.htm )

            return (
                n41 * (
                    +n14 * n23 * n32
                     - n13 * n24 * n32
                     - n14 * n22 * n33
                     + n12 * n24 * n33
                     + n13 * n22 * n34
                     - n12 * n23 * n34
                ) +
                n42 * (
                    +n11 * n23 * n34
                     - n11 * n24 * n33
                     + n14 * n21 * n33
                     - n13 * n21 * n34
                     + n13 * n24 * n31
                     - n14 * n23 * n31
                ) +
                n43 * (
                    +n11 * n24 * n32
                     - n11 * n22 * n34
                     - n14 * n21 * n32
                     + n12 * n21 * n34
                     + n14 * n22 * n31
                     - n12 * n24 * n31
                ) +
                n44 * (
                    -n13 * n22 * n31
                     - n11 * n23 * n32
                     + n11 * n22 * n33
                     + n13 * n21 * n32
                     - n12 * n21 * n33
                     + n12 * n23 * n31
                )

            );
        }

        public Matrix4d Transpose()
        {
            var te = this.Elements;
            double tmp;

            tmp = te[1]; te[1] = te[4]; te[4] = tmp;
            tmp = te[2]; te[2] = te[8]; te[8] = tmp;
            tmp = te[6]; te[6] = te[9]; te[9] = tmp;

            tmp = te[3]; te[3] = te[12]; te[12] = tmp;
            tmp = te[7]; te[7] = te[13]; te[13] = tmp;
            tmp = te[11]; te[11] = te[14]; te[14] = tmp;

            return this;
        }

        public Matrix4d Transposed()
        {
            Matrix4d m = Matrix4d.Identity();

            m.Copy(this);

            return m.Transpose();
        }

        public Matrix4d SetPosition(Vector3 x)
        {
             var te = this.Elements;

             te[12] = x.X;
             te[13] = x.Y;
             te[14] = x.Z;

             return this;
        }
        public Matrix4d SetPosition(double x, double y, double z)
        {
            var te = this.Elements;

            te[12] = x;
            te[13] = y;
            te[14] = z;

            return this;
        }
        public Matrix4d Invert()
        {
            // based on http://www.euclideanspace.com/maths/algebra/matrix/functions/inverse/fourD/index.htm
            var te = this.Elements;

            double n11 = te[0], n21 = te[1], n31 = te[2], n41 = te[3],
                n12 = te[4], n22 = te[5], n32 = te[6], n42 = te[7],
                n13 = te[8], n23 = te[9], n33 = te[10], n43 = te[11],
                n14 = te[12], n24 = te[13], n34 = te[14], n44 = te[15],

                t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44,
                t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44,
                t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44,
                t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

            var det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;

            if (det == 0)
            {

                var msg = "THREE.Matrix4: .GetInverse() can't invert matrix, determinant is 0";

                Trace.TraceError(msg);

                return this.Set(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            }

            var detInv = 1 / det;

            te[0] = t11 * detInv;
            te[1] = (n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44) * detInv;
            te[2] = (n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44) * detInv;
            te[3] = (n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43) * detInv;

            te[4] = t12 * detInv;
            te[5] = (n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44) * detInv;
            te[6] = (n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44) * detInv;
            te[7] = (n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43) * detInv;

            te[8] = t13 * detInv;
            te[9] = (n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44) * detInv;
            te[10] = (n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44) * detInv;
            te[11] = (n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43) * detInv;

            te[12] = t14 * detInv;
            te[13] = (n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34) * detInv;
            te[14] = (n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34) * detInv;
            te[15] = (n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33) * detInv;

            return this;
        }
        public Matrix4d GetInverse(Matrix4d m)
        {
            // based on http://www.euclideanspace.com/maths/algebra/matrix/functions/inverse/fourD/index.htm
		    var te = this.Elements;
			var me = m.Elements;

			double n11 = me[ 0 ], n21 = me[ 1 ], n31 = me[ 2 ], n41 = me[ 3 ],
			    n12 = me[ 4 ], n22 = me[ 5 ], n32 = me[ 6 ], n42 = me[ 7 ],
			    n13 = me[ 8 ], n23 = me[ 9 ], n33 = me[ 10 ], n43 = me[ 11 ],
			    n14 = me[ 12 ], n24 = me[ 13 ], n34 = me[ 14 ], n44 = me[ 15 ],

			    t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44,
			    t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44,
			    t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44,
			    t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

		    var det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;

		    if ( det == 0 ) {

			    var msg = "THREE.Matrix4d: .GetInverse() can't invert matrix, determinant is 0";

                Trace.TraceError(msg);

                return Matrix4d.Identity();

		    }

		    var detInv = 1 / det;

		    te[ 0 ] = t11 * detInv;
		    te[ 1 ] = ( n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44 ) * detInv;
		    te[ 2 ] = ( n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44 ) * detInv;
		    te[ 3 ] = ( n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43 ) * detInv;

		    te[ 4 ] = t12 * detInv;
		    te[ 5 ] = ( n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44 ) * detInv;
		    te[ 6 ] = ( n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44 ) * detInv;
		    te[ 7 ] = ( n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43 ) * detInv;

		    te[ 8 ] = t13 * detInv;
		    te[ 9 ] = ( n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44 ) * detInv;
		    te[ 10 ] = ( n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44 ) * detInv;
		    te[ 11 ] = ( n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43 ) * detInv;

		    te[ 12 ] = t14 * detInv;
		    te[ 13 ] = ( n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34 ) * detInv;
		    te[ 14 ] = ( n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34 ) * detInv;
		    te[ 15 ] = ( n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33 ) * detInv;

		    return this;
        }

        public Matrix4d Inverted()
        {
            Matrix4d r = new Matrix4d();
            
		    // based on http://www.euclideanspace.com/maths/algebra/matrix/functions/inverse/fourD/index.htm
            var te = r.Elements;
			var me = this.Elements;

			double n11 = me[ 0 ], n21 = me[ 1 ], n31 = me[ 2 ], n41 = me[ 3 ],
			n12 = me[ 4 ], n22 = me[ 5 ], n32 = me[ 6 ], n42 = me[ 7 ],
			n13 = me[ 8 ], n23 = me[ 9 ], n33 = me[ 10 ], n43 = me[ 11 ],
			n14 = me[ 12 ], n24 = me[ 13 ], n34 = me[ 14 ], n44 = me[ 15 ],

			t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44,
			t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44,
			t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44,
			t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

		    double det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;

		    if ( det == 0 ) {

                return Matrix4d.Identity();

		    }

		    var detInv = 1 / det;

		    te[ 0 ] = t11 * detInv;
		    te[ 1 ] = ( n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44 ) * detInv;
		    te[ 2 ] = ( n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44 ) * detInv;
		    te[ 3 ] = ( n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43 ) * detInv;

		    te[ 4 ] = t12 * detInv;
		    te[ 5 ] = ( n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44 ) * detInv;
		    te[ 6 ] = ( n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44 ) * detInv;
		    te[ 7 ] = ( n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43 ) * detInv;

		    te[ 8 ] = t13 * detInv;
		    te[ 9 ] = ( n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44 ) * detInv;
		    te[ 10 ] = ( n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44 ) * detInv;
		    te[ 11 ] = ( n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43 ) * detInv;

		    te[ 12 ] = t14 * detInv;
		    te[ 13 ] = ( n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34 ) * detInv;
		    te[ 14 ] = ( n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34 ) * detInv;
		    te[ 15 ] = ( n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33 ) * detInv;
                      

            return r;
        }

        public Matrix4d Scale(Vector3 v)
        {
            var te = this.Elements;
            double x = v.X, y = v.Y, z = v.Z;

            te[0] *= x; te[4] *= y; te[8] *= z;
            te[1] *= x; te[5] *= y; te[9] *= z;
            te[2] *= x; te[6] *= y; te[10] *= z;
            te[3] *= x; te[7] *= y; te[11] *= z;

            return this;
        }

        public double GetMaxScaleOnAxis()
        {
            var te = this.Elements;

            var scaleXSq = te[0] * te[0] + te[1] * te[1] + te[2] * te[2];
            var scaleYSq = te[4] * te[4] + te[5] * te[5] + te[6] * te[6];
            var scaleZSq = te[8] * te[8] + te[9] * te[9] + te[10] * te[10];

            return System.Math.Sqrt(System.Math.Max(scaleXSq,System.Math.Max(scaleYSq, scaleZSq)));
        }

        public Matrix4d MakeTranslation(double x, double y, double z)
        {
            this.Set(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1
            );

            return this;
        }

        public static Matrix4d CreateTranslation(double x, double y, double z)
        {
            Matrix4d m = Matrix4d.Identity();

            m.Set(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1
            );

            return m;
        }
	    public Matrix4d MakeRotationX(double theta )
        {

		    double c = System.Math.Cos( theta );
            double s = System.Math.Sin( theta );

		    this.Set(

			    1, 0, 0, 0,
			    0, c, - s, 0,
			    0, s, c, 0,
			    0, 0, 0, 1

		    );

		    return this;
	    }

	    public Matrix4d MakeRotationY(double theta )
        {

		    double c = System.Math.Cos( theta );
            double s = System.Math.Sin( theta );

		    this.Set(

			     c, 0, s, 0,
			     0, 1, 0, 0,
			    - s, 0, c, 0,
			     0, 0, 0, 1

		    );

		    return this;

	    }

	    public Matrix4d MakeRotationZ(double theta ) 
        {

		    double c = System.Math.Cos( theta );
            double s = System.Math.Sin( theta );

		    this.Set(
			    c, - s, 0, 0,
			    s, c, 0, 0,
			    0, 0, 1, 0,
			    0, 0, 0, 1
		    );

		    return this;

	    }

	    public Matrix4d MakeRotationAxis(Vector3 axis, double angle ) 
        {

		    // Based on http://www.gamedev.net/reference/articles/article1199.asp

		    var c = System.Math.Cos( angle );
		    var s = System.Math.Sin( angle );
		    var t = 1 - c;
		    double x = axis.X, y = axis.Y, z = axis.Z;
		    double tx = t * x, ty = t * y;

		    this.Set(

			    tx * x + c, tx * y - s * z, tx * z + s * y, 0,
			    tx * y + s * z, ty * y + c, ty * z - s * x, 0,
			    tx * z - s * y, ty * z + s * x, t * z * z + c, 0,
			    0, 0, 0, 1

		    );

		     return this;

	    }

	    public Matrix4d MakeScale(double x, double y, double z ) 
        {

		    this.Set(

			    x, 0, 0, 0,
			    0, y, 0, 0,
			    0, 0, z, 0,
			    0, 0, 0, 1

		    );

		    return this;

	    }

	    public Matrix4d MakeShear(double x, double y, double z )
        {

		    this.Set(

			    1, y, z, 0,
			    x, 1, z, 0,
			    x, y, 1, 0,
			    0, 0, 0, 1

		    );

		    return this;

	    }

	    public Matrix4d Compose(Vector3d position, Quaterniond quaternion, Vector3d scale )
        {

		    var te = this.Elements;

		    double x = quaternion.X, y = quaternion.Y, z = quaternion.Z, w = quaternion.W;
		    double x2 = x + x,	y2 = y + y, z2 = z + z;
		    double xx = x * x2, xy = x * y2, xz = x * z2;
		    double yy = y * y2, yz = y * z2, zz = z * z2;
		    double wx = w * x2, wy = w * y2, wz = w * z2;

		    double sx = scale.X, sy = scale.Y, sz = scale.Z;

		    te[ 0 ] = ( 1 - ( yy + zz ) ) * sx;
		    te[ 1 ] = ( xy + wz ) * sx;
		    te[ 2 ] = ( xz - wy ) * sx;
		    te[ 3 ] = 0;

		    te[ 4 ] = ( xy - wz ) * sy;
		    te[ 5 ] = ( 1 - ( xx + zz ) ) * sy;
		    te[ 6 ] = ( yz + wx ) * sy;
		    te[ 7 ] = 0;

		    te[ 8 ] = ( xz + wy ) * sz;
		    te[ 9 ] = ( yz - wx ) * sz;
		    te[ 10 ] = ( 1 - ( xx + yy ) ) * sz;
		    te[ 11 ] = 0;

		    te[ 12 ] = position.X;
		    te[ 13 ] = position.Y;
		    te[ 14 ] = position.Z;
		    te[ 15 ] = 1;

		    return this;

	    }

	    public Matrix4d Decompose(Vector3d position, Quaterniond quaternion, Vector3d scale ) 
        {

            Vector3d _v1 = Vector3d.Zero();
            Matrix4d _m1 = Matrix4d.Identity();
            var te = this.Elements;
            
		    var sx = _v1.Set( te[ 0 ], te[ 1 ], te[ 2 ] ).Length();
		    var sy = _v1.Set( te[ 4 ], te[ 5 ], te[ 6 ] ).Length();
		    var sz = _v1.Set( te[ 8 ], te[ 9 ], te[ 10 ] ).Length();

		    // if determine is negative, we need to invert one scale
		    var det = this.Determinant();
		    if ( det < 0 ) sx = - sx;

		    position.X = te[ 12 ];
		    position.Y = te[ 13 ];
		    position.Z = te[ 14 ];

		    // scale the rotation part
		    _m1.Copy( this );

		    var invSX = 1 / sx;
		    var invSY = 1 / sy;
		    var invSZ = 1 / sz;

		    _m1.Elements[ 0 ] *= invSX;
		    _m1.Elements[ 1 ] *= invSX;
		    _m1.Elements[ 2 ] *= invSX;

		    _m1.Elements[ 4 ] *= invSY;
		    _m1.Elements[ 5 ] *= invSY;
		    _m1.Elements[ 6 ] *= invSY;

		    _m1.Elements[ 8 ] *= invSZ;
		    _m1.Elements[ 9 ] *= invSZ;
		    _m1.Elements[ 10 ] *= invSZ;

		    quaternion.SetFromRotationMatrix( _m1 );

		    scale.X = sx;
		    scale.Y = sy;
		    scale.Z = sz;

		    return this;

	    }

	    public Matrix4d MakePerspective(double left, double right, double top, double bottom, double near, double far ) 
        {

		    var te = this.Elements;
		    var x = 2.0f * near / ( right - left );
		    var y = 2.0f * near / ( top - bottom );

		    var a = ( right + left ) / ( right - left );
		    var b = ( top + bottom ) / ( top - bottom );
		    var c = - ( far + near ) / ( far - near );
		    var d = - 2.0f * far * near / ( far - near );

		    te[ 0 ] = x;	te[ 4 ] = 0;	te[ 8 ] = a;	te[ 12 ] = 0;
		    te[ 1 ] = 0;	te[ 5 ] = y;	te[ 9 ] = b;	te[ 13 ] = 0;
		    te[ 2 ] = 0;	te[ 6 ] = 0;	te[ 10 ] = c;	te[ 14 ] = d;
		    te[ 3 ] = 0;	te[ 7 ] = 0;	te[ 11 ] = -1;	te[ 15 ] = 0;

		    return this;

	    }
        public Matrix4d MakePerspectiveFieldOfView(double fovy,double aspect,double depthNear,double depthFar)
        {
            var maxY = depthNear * System.Math.Tan(0.5f * fovy);
            var minY = -maxY;
            var minX = minY * aspect;
            var maxX = maxY * aspect;

            return MakePerspective(minX, maxX, minY, maxY, depthNear, depthFar);
        }
	    public Matrix4d MakeOrthographic(double left, double right, double top, double bottom, double near, double far ) 
        {

		    var te = this.Elements;
		    var w = 1.0f / ( right - left );
		    var h = 1.0f / ( top - bottom );
		    var p = 1.0f / ( far - near );

		    var x = ( right + left ) * w;
		    var y = ( top + bottom ) * h;
		    var z = ( far + near ) * p;

		    te[ 0 ] = 2 * w;	te[ 4 ] = 0;	te[ 8 ] = 0;	te[ 12 ] = - x;
		    te[ 1 ] = 0;	te[ 5 ] = 2 * h;	te[ 9 ] = 0;	te[ 13 ] = - y;
		    te[ 2 ] = 0;	te[ 6 ] = 0;	te[ 10 ] = - 2 * p;	te[ 14 ] = - z;
		    te[ 3 ] = 0;	te[ 7 ] = 0;	te[ 11 ] = 0;	te[ 15 ] = 1;

		    return this;

	    }

        public bool Equals(Matrix4d matrix ) 
        {
		    var te = this.Elements;
		    var me = matrix.Elements;

		    for ( var i = 0; i < 16; i ++ ) {

			    if ( te[ i ] != me[ i ] ) return false;

		    }

		    return true;
	    }
        public Matrix4d FromArray(double[] array, int? offset=null ) 
        {
            int index = 0;
		    if ( offset != null ) index = (int)offset;

		    for ( var i = 0; i < 16; i ++ ) {

			    this.Elements[ i ] = array[ i + index ];

		    }

		    return this;

	    }

	    public double[] ToArray(double[] array=null, int? offset=null ) {

            int index = 0;
            if (array == null) array = new double[16];
            if (offset != null) index = (int)offset;

		    var te = this.Elements;

		    array[ index ] = te[ 0 ];
		    array[ index + 1 ] = te[ 1 ];
		    array[ index + 2 ] = te[ 2 ];
		    array[ index + 3 ] = te[ 3 ];

		    array[ index + 4 ] = te[ 4 ];
		    array[ index + 5 ] = te[ 5 ];
		    array[ index + 6 ] = te[ 6 ];
		    array[ index + 7 ] = te[ 7 ];

		    array[ index + 8 ] = te[ 8 ];
		    array[ index + 9 ] = te[ 9 ];
		    array[ index + 10 ] = te[ 10 ];
		    array[ index + 11 ] = te[ 11 ];

		    array[ index + 12 ] = te[ 12 ];
		    array[ index + 13 ] = te[ 13 ];
		    array[ index + 14 ] = te[ 14 ];
		    array[ index + 15 ] = te[ 15 ];

		    return array;

	    }
    }
}
