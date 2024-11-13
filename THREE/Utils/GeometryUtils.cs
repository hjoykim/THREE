using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace THREE
{
    [Serializable]
    public class GeometryUtils
    {
        public static List<Vector3> Hilbert3D(Vector3 center=null, float size = 10, int iterations = 1, int v0 = 0, int v1 = 1, int v2 = 2, int v3 = 3, int v4 = 4, int v5 = 5, int v6 = 6, int v7 = 7)
        {
            if (center == null) center = Vector3.Zero();
            float half = size / 2;
            List<Vector3> vec_s = new List<Vector3> {
            new Vector3( center.X - half, center.Y + half, center.Z - half ),
            new Vector3( center.X - half, center.Y + half, center.Z + half ),
            new Vector3( center.X - half, center.Y - half, center.Z + half ),
            new Vector3( center.X - half, center.Y - half, center.Z - half ),
            new Vector3( center.X + half, center.Y - half, center.Z - half ),
            new Vector3( center.X + half, center.Y - half, center.Z + half ),
            new Vector3( center.X + half, center.Y + half, center.Z + half ),
            new Vector3( center.X + half, center.Y + half, center.Z - half )
            };

            List<Vector3> vec = new List<Vector3>{
                vec_s[ v0 ],
                vec_s[ v1 ],
                vec_s[ v2 ],
                vec_s[ v3 ],
                vec_s[ v4 ],
                vec_s[ v5 ],
                vec_s[ v6 ],
                vec_s[ v7 ]
            };

            // Recurse iterations
            if (--iterations >= 0)
            {

                List<Vector3> tmp = new List<Vector3>();

                tmp.AddRange(Hilbert3D(vec[0], half, iterations, v0, v3, v4, v7, v6, v5, v2, v1));
                tmp.AddRange(Hilbert3D(vec[1], half, iterations, v0, v7, v6, v1, v2, v5, v4, v3));
                tmp.AddRange(Hilbert3D(vec[2], half, iterations, v0, v7, v6, v1, v2, v5, v4, v3));
                tmp.AddRange(Hilbert3D(vec[3], half, iterations, v2, v3, v0, v1, v6, v7, v4, v5));
                tmp.AddRange(Hilbert3D(vec[4], half, iterations, v2, v3, v0, v1, v6, v7, v4, v5));
                tmp.AddRange(Hilbert3D(vec[5], half, iterations, v4, v3, v2, v5, v6, v1, v0, v7));
                tmp.AddRange(Hilbert3D(vec[6], half, iterations, v4, v3, v2, v5, v6, v1, v0, v7));
                tmp.AddRange(Hilbert3D(vec[7], half, iterations, v6, v5, v2, v1, v0, v3, v4, v7));

                // Return recursive call
                return tmp;

            }

            // Return complete Hilbert Curve.
            return vec;
        }
    }
}
