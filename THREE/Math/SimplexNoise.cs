﻿
namespace THREE
{
    [Serializable]
    public class SimplexNoise
    {
        float[][] grad3;
        float[][] grad4;
        int[] p = new int[256];
        int[] perm = new int[512];

        float[][] simplex;

        public SimplexNoise()
        {
            this.grad3 = new float[][]  {
                new float[]{1, 1, 0},
                new float[]{ -1, 1, 0 },
                new float[]{ 1, -1, 0 },
                new float[] { -1, -1, 0 },
                new float[]{ 1, 0, 1 },
                new float[]{ -1, 0, 1 },
                new float[] { 1, 0, -1 },
                new float[] { -1, 0, -1 },
                new float[] { 0, 1, 1 },
                new float[] { 0, -1, 1 },
                new float[] { 0, 1, -1 },
                new float[] { 0, -1, -1 }
            };

            this.grad4 = new float[][]
                {
                    new float[] {0, 1, 1, 1}, new float[] { 0, 1, 1, -1 }, new float[] { 0, 1, -1, 1 }, new float[] { 0, 1, -1, -1 },
                 new float[] { 0, -1, 1, 1 }, new float[] { 0, -1, 1, -1 }, new float[] { 0, -1, -1, 1 }, new float[] { 0, -1, -1, -1 },
                 new float[] { 1, 0, 1, 1 }, new float[] { 1, 0, 1, -1 }, new float[] { 1, 0, -1, 1 }, new float[] { 1, 0, -1, -1 },
                 new float[] { -1, 0, 1, 1 }, new float[] { -1, 0, 1, -1 }, new float[] { -1, 0, -1, 1 }, new float[] { -1, 0, -1, -1 },
                 new float[] { 1, 1, 0, 1 }, new float[] { 1, 1, 0, -1 }, new float[] { 1, -1, 0, 1 }, new float[] { 1, -1, 0, -1 },
                 new float[] { -1, 1, 0, 1 }, new float[] { -1, 1, 0, -1 }, new float[] { -1, -1, 0, 1 }, new float[] { -1, -1, 0, -1 },
                 new float[] { 1, 1, 1, 0 }, new float[] { 1, 1, -1, 0 }, new float[] { 1, -1, 1, 0 }, new float[] { 1, -1, -1, 0 },
                 new float[] { -1, 1, 1, 0 }, new float[] { -1, 1, -1, 0 }, new float[] { -1, -1, 1, 0 }, new float[] { -1, -1, -1, 0 }
                };
            for (var i = 0; i < 256; i++)
            {
                this.p[i] = (int)System.Math.Floor(MathUtils.random.NextDouble() * 256);
            }

            for (var i = 0; i < 512; i++)
            {

                this.perm[i] = this.p[i & 255];

            }

            // A lookup table to traverse the simplex around a given point in 4D.
            // Details can be found where this table is used, in the 4D noise method.
            this.simplex = new float[][]
                {

                    new float[] {0, 1, 2, 3}, new float[] { 0, 1, 3, 2 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 2, 3, 1 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 1, 2, 3, 0 },
                    new float[] { 0, 2, 1, 3 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 3, 1, 2 }, new float[] { 0, 3, 2, 1 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 1, 3, 2, 0 },
                    new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 },
                    new float[] { 1, 2, 0, 3 }, new float[] { 0, 0, 0, 0 }, new float[] { 1, 3, 0, 2 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 2, 3, 0, 1 }, new float[] { 2, 3, 1, 0 },
                    new float[] { 1, 0, 2, 3 }, new float[] { 1, 0, 3, 2 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 2, 0, 3, 1 }, new float[] { 0, 0, 0, 0 }, new float[] { 2, 1, 3, 0 },
                    new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 },
                    new float[] { 2, 0, 1, 3 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 3, 0, 1, 2 }, new float[] { 3, 0, 2, 1 }, new float[] { 0, 0, 0, 0 }, new float[] { 3, 1, 2, 0 },
                    new float[] { 2, 1, 0, 3 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 3, 1, 0, 2 }, new float[] { 0, 0, 0, 0 }, new float[] { 3, 2, 0, 1 }, new float[] { 3, 2, 1, 0 }
                };
        }

        private float Dot(float[] g, float x, float y)
        {
            return g[0] * x + g[1] * y;
        }

        private float Dot3(float[] g, float x, float y, float z)
        {
            return g[0] * x + g[1] * y + g[2] * z;
        }

        private float Dot4(float[] g, float x, float y, float z, float w)
        {
            return g[0] * x + g[1] * y + g[2] * z + g[3] * w;
        }

        public float Noise(float xin, float yin)
        {
            float n0, n1, n2; // Noise contributions from the three corners
                              // Skew the input space to determine which simplex cell we're in
            var F2 = 0.5f * (float)(System.Math.Sqrt(3.0) - 1.0);
            var s = (xin + yin) * F2; // Hairy factor for 2D
            var i = (int)System.Math.Floor(xin + s);
            var j = (int)System.Math.Floor(yin + s);
            var G2 = (float)(3.0f - System.Math.Sqrt(3.0f)) / 6.0f;
            var t = (i + j) * G2;
            var X0 = i - t; // Unskew the cell origin back to (x,y) space
            var Y0 = j - t;
            float x0 = xin - X0; // The x,y distances from the cell origin
            float y0 = yin - Y0;
            // For the 2D case, the simplex shape is an equilateral triangle.
            // Determine which simplex we are in.
            int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
            if (x0 > y0)
            {

                i1 = 1; j1 = 0;

                // lower triangle, XY order: (0,0)->(1,0)->(1,1)

            }
            else
            {

                i1 = 0; j1 = 1;

            } // upper triangle, YX order: (0,0)->(0,1)->(1,1)

            // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
            // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
            // c = (3-sqrt(3))/6
            var x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
            var y1 = y0 - j1 + G2;
            var x2 = x0 - 1.0f + 2.0f * G2; // Offsets for last corner in (x,y) unskewed coords
            var y2 = y0 - 1.0f + 2.0f * G2;
            // Work out the hashed gradient indices of the three simplex corners
            var ii = i & 255;
            var jj = j & 255;
            int gi0 = this.perm[ii + this.perm[jj]] % 12;
            int gi1 = this.perm[ii + i1 + this.perm[jj + j1]] % 12;
            int gi2 = this.perm[ii + 1 + this.perm[jj + 1]] % 12;
            // Calculate the contribution from the three corners
            float t0 = 0.5f - x0 * x0 - y0 * y0;
            if (t0 < 0) n0 = 0.0f;
            else
            {

                t0 *= t0;
                n0 = t0 * t0 * this.Dot(this.grad3[gi0], x0, y0); // (x,y) of grad3 used for 2D gradient

            }

            float t1 = 0.5f - x1 * x1 - y1 * y1;
            if (t1 < 0) n1 = 0.0f;
            else
            {

                t1 *= t1;
                n1 = t1 * t1 * this.Dot(this.grad3[gi1], x1, y1);

            }

            float t2 = 0.5f - x2 * x2 - y2 * y2;
            if (t2 < 0) n2 = 0.0f;
            else
            {

                t2 *= t2;
                n2 = t2 * t2 * this.Dot(this.grad3[gi2], x2, y2);

            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [-1,1].
            return 70.0f * (n0 + n1 + n2);
        }

        public float Noise3d(float xin, float yin, float zin)
        {
            float n0, n1, n2, n3; // Noise contributions from the four corners
                                  // Skew the input space to determine which simplex cell we're in
            float F3 = 1.0f / 3.0f;
            float s = (xin + yin + zin) * F3; // Very nice and simple skew factor for 3D
            int i = (int)System.Math.Floor(xin + s);
            int j = (int)System.Math.Floor(yin + s);
            int k = (int)System.Math.Floor(zin + s);
            float G3 = 1.0f / 6.0f; // Very nice and simple unskew factor, too
            float t = (i + j + k) * G3;
            float X0 = i - t; // Unskew the cell origin back to (x,y,z) space
            float Y0 = j - t;
            float Z0 = k - t;
            float x0 = xin - X0; // The x,y,z distances from the cell origin
            float y0 = yin - Y0;
            float z0 = zin - Z0;
            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
            int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords
            if (x0 >= y0)
            {

                if (y0 >= z0)
                {

                    i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0;

                    // X Y Z order

                }
                else if (x0 >= z0)
                {

                    i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1;

                    // X Z Y order

                }
                else
                {

                    i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1;

                } // Z X Y order

            }
            else
            { // x0<y0

                if (y0 < z0)
                {

                    i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1;

                    // Z Y X order

                }
                else if (x0 < z0)
                {

                    i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1;

                    // Y Z X order

                }
                else
                {

                    i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0;

                } // Y X Z order

            }

            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.
            var x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
            var y1 = y0 - j1 + G3;
            var z1 = z0 - k1 + G3;
            var x2 = x0 - i2 + 2.0f * G3; // Offsets for third corner in (x,y,z) coords
            var y2 = y0 - j2 + 2.0f * G3;
            var z2 = z0 - k2 + 2.0f * G3;
            var x3 = x0 - 1.0f + 3.0f * G3; // Offsets for last corner in (x,y,z) coords
            var y3 = y0 - 1.0f + 3.0f * G3;
            var z3 = z0 - 1.0f + 3.0f * G3;
            // Work out the hashed gradient indices of the four simplex corners
            var ii = i & 255;
            var jj = j & 255;
            var kk = k & 255;
            var gi0 = this.perm[ii + this.perm[jj + this.perm[kk]]] % 12;
            var gi1 = this.perm[ii + i1 + this.perm[jj + j1 + this.perm[kk + k1]]] % 12;
            var gi2 = this.perm[ii + i2 + this.perm[jj + j2 + this.perm[kk + k2]]] % 12;
            var gi3 = this.perm[ii + 1 + this.perm[jj + 1 + this.perm[kk + 1]]] % 12;
            // Calculate the contribution from the four corners
            float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 < 0) n0 = 0.0f;
            else
            {

                t0 *= t0;
                n0 = t0 * t0 * this.Dot3(this.grad3[gi0], x0, y0, z0);

            }

            var t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 < 0) n1 = 0.0f;
            else
            {

                t1 *= t1;
                n1 = t1 * t1 * this.Dot3(this.grad3[gi1], x1, y1, z1);

            }

            var t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 < 0) n2 = 0.0f;
            else
            {

                t2 *= t2;
                n2 = t2 * t2 * this.Dot3(this.grad3[gi2], x2, y2, z2);

            }

            var t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 < 0) n3 = 0.0f;
            else
            {

                t3 *= t3;
                n3 = t3 * t3 * this.Dot3(this.grad3[gi3], x3, y3, z3);

            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1]
            return 32.0f * (n0 + n1 + n2 + n3);
        }
        public float Noise4d(float x, float y, float z, float w)
        {

            // The skewing and unskewing factors are hairy again for the 4D case
            float F4 = (float)(System.Math.Sqrt(5.0f) - 1.0f) / 4.0f;
            float G4 = (5.0f - (float)System.Math.Sqrt(5.0f)) / 20.0f;
            float n0, n1, n2, n3, n4; // Noise contributions from the five corners
                                      // Skew the (x,y,z,w) space to determine which cell of 24 simplices we're in
            float s = (x + y + z + w) * F4; // Factor for 4D skewing
            int i = (int)System.Math.Floor(x + s);
            int j = (int)System.Math.Floor(y + s);
            int k = (int)System.Math.Floor(z + s);
            var l = (int)System.Math.Floor(w + s);
            var t = (i + j + k + l) * G4; // Factor for 4D unskewing
            var X0 = i - t; // Unskew the cell origin back to (x,y,z,w) space
            var Y0 = j - t;
            var Z0 = k - t;
            var W0 = l - t;
            var x0 = x - X0; // The x,y,z,w distances from the cell origin
            var y0 = y - Y0;
            var z0 = z - Z0;
            var w0 = w - W0;

            // For the 4D case, the simplex is a 4D shape I won't even try to describe.
            // To find out which of the 24 possible simplices we're in, we need to
            // determine the magnitude ordering of x0, y0, z0 and w0.
            // The method below is a good way of finding the ordering of x,y,z,w and
            // then find the correct traversal order for the simplex we’re in.
            // First, six pair-wise comparisons are performed between each possible pair
            // of the four coordinates, and the results are used to add up binary bits
            // for an integer index.
            var c1 = (x0 > y0) ? 32 : 0;
            var c2 = (x0 > z0) ? 16 : 0;
            var c3 = (y0 > z0) ? 8 : 0;
            var c4 = (x0 > w0) ? 4 : 0;
            var c5 = (y0 > w0) ? 2 : 0;
            var c6 = (z0 > w0) ? 1 : 0;
            var c = c1 + c2 + c3 + c4 + c5 + c6;
            int i1, j1, k1, l1; // The integer offsets for the second simplex corner
            int i2, j2, k2, l2; // The integer offsets for the third simplex corner
            int i3, j3, k3, l3; // The integer offsets for the fourth simplex corner
                                // simplex[c] is a 4-vector with the numbers 0, 1, 2 and 3 in some order.
                                // Many values of c will never occur, since e.g. x>y>z>w makes x<z, y<w and x<w
                                // impossible. Only the 24 indices which have non-zero entries make any sense.
                                // We use a thresholding to set the coordinates in turn from the largest magnitude.
                                // The number 3 in the "simplex" array is at the position of the largest coordinate.
            i1 = simplex[c][0] >= 3 ? 1 : 0;
            j1 = simplex[c][1] >= 3 ? 1 : 0;
            k1 = simplex[c][2] >= 3 ? 1 : 0;
            l1 = simplex[c][3] >= 3 ? 1 : 0;
            // The number 2 in the "simplex" array is at the second largest coordinate.
            i2 = simplex[c][0] >= 2 ? 1 : 0;
            j2 = simplex[c][1] >= 2 ? 1 : 0; k2 = simplex[c][2] >= 2 ? 1 : 0;
            l2 = simplex[c][3] >= 2 ? 1 : 0;
            // The number 1 in the "simplex" array is at the second smallest coordinate.
            i3 = simplex[c][0] >= 1 ? 1 : 0;
            j3 = simplex[c][1] >= 1 ? 1 : 0;
            k3 = simplex[c][2] >= 1 ? 1 : 0;
            l3 = simplex[c][3] >= 1 ? 1 : 0;
            // The fifth corner has all coordinate offsets = 1, so no need to look that up.
            var x1 = x0 - i1 + G4; // Offsets for second corner in (x,y,z,w) coords
            var y1 = y0 - j1 + G4;
            var z1 = z0 - k1 + G4;
            var w1 = w0 - l1 + G4;
            var x2 = x0 - i2 + 2.0f * G4; // Offsets for third corner in (x,y,z,w) coords
            var y2 = y0 - j2 + 2.0f * G4;
            var z2 = z0 - k2 + 2.0f * G4;
            var w2 = w0 - l2 + 2.0f * G4;
            var x3 = x0 - i3 + 3.0f * G4; // Offsets for fourth corner in (x,y,z,w) coords
            var y3 = y0 - j3 + 3.0f * G4;
            var z3 = z0 - k3 + 3.0f * G4;
            var w3 = w0 - l3 + 3.0f * G4;
            var x4 = x0 - 1.0f + 4.0f * G4; // Offsets for last corner in (x,y,z,w) coords
            var y4 = y0 - 1.0f + 4.0f * G4;
            var z4 = z0 - 1.0f + 4.0f * G4;
            var w4 = w0 - 1.0f + 4.0f * G4;
            // Work out the hashed gradient indices of the five simplex corners
            var ii = i & 255;
            var jj = j & 255;
            var kk = k & 255;
            var ll = l & 255;
            var gi0 = perm[ii + perm[jj + perm[kk + perm[ll]]]] % 32;
            var gi1 = perm[ii + i1 + perm[jj + j1 + perm[kk + k1 + perm[ll + l1]]]] % 32;
            var gi2 = perm[ii + i2 + perm[jj + j2 + perm[kk + k2 + perm[ll + l2]]]] % 32;
            var gi3 = perm[ii + i3 + perm[jj + j3 + perm[kk + k3 + perm[ll + l3]]]] % 32;
            var gi4 = perm[ii + 1 + perm[jj + 1 + perm[kk + 1 + perm[ll + 1]]]] % 32;
            // Calculate the contribution from the five corners
            var t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t0 < 0) n0 = 0.0f;
            else
            {

                t0 *= t0;
                n0 = t0 * t0 * this.Dot4(grad4[gi0], x0, y0, z0, w0);

            }

            var t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t1 < 0) n1 = 0.0f;
            else
            {

                t1 *= t1;
                n1 = t1 * t1 * this.Dot4(grad4[gi1], x1, y1, z1, w1);

            }

            var t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t2 < 0) n2 = 0.0f;
            else
            {

                t2 *= t2;
                n2 = t2 * t2 * this.Dot4(grad4[gi2], x2, y2, z2, w2);

            }

            var t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t3 < 0) n3 = 0.0f;
            else
            {

                t3 *= t3;
                n3 = t3 * t3 * this.Dot4(grad4[gi3], x3, y3, z3, w3);

            }

            var t4 = 0.6f - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t4 < 0) n4 = 0.0f;
            else
            {

                t4 *= t4;
                n4 = t4 * t4 * this.Dot4(grad4[gi4], x4, y4, z4, w4);

            }

            // Sum up and scale the result to cover the range [-1,1]
            return 27.0f * (n0 + n1 + n2 + n3 + n4);
        }
    }
}
